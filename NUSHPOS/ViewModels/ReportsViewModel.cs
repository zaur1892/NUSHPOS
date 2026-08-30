using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System.Threading.Tasks;
using Dapper;
using System;
using Microsoft.Extensions.DependencyInjection;
using ClosedXML.Excel;
using System.IO;
using System.Diagnostics;

namespace NUSHPOS.ViewModels;

public partial class ReportsViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly DatabaseService _databaseService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<ReportCategoryModel> _categories = new();

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<ReportTabModel> _openReports = new();

    [ObservableProperty]
    private ReportTabModel? _selectedReport;

    private bool _isEditingReport;
    public bool IsEditingReport
    {
        get => _isEditingReport;
        set => SetProperty(ref _isEditingReport, value);
    }

    private ReportEditorViewModel? _editorViewModel;
    public ReportEditorViewModel? EditorViewModel
    {
        get => _editorViewModel;
        set => SetProperty(ref _editorViewModel, value);
    }

    public ReportsViewModel(NavigationService navigationService, DatabaseService databaseService, IServiceProvider serviceProvider)
    {
        _navigationService = navigationService;
        _databaseService = databaseService;
        _serviceProvider = serviceProvider;
        Title = "Raporlar";
        _ = InitializeReportsAsync();
    }

    [RelayCommand]
    private async Task NewReportAsync()
    {
        var editor = ServiceProviderServiceExtensions.GetRequiredService<ReportEditorViewModel>(_serviceProvider);
        editor.IsNewReport = true;
        await editor.LoadCategoriesAsync();
        System.Windows.Application.Current.Dispatcher.Invoke(() => {
            var window = new System.Windows.Window
            {
                Title = "RAPOR DÜZENLEYİCİ",
                Content = new NUSHPOS.Views.ReportEditorView { DataContext = editor },
                Width = 1000,
                Height = 700,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };
            editor.RequestClose += (s, e) => window.Close();
            window.ShowDialog();
        });
    }

    [RelayCommand]
    private async Task EditReportAsync()
    {
        if (SelectedReport == null || SelectedReport.Report == null) return;
        var editor = ServiceProviderServiceExtensions.GetRequiredService<ReportEditorViewModel>(_serviceProvider);
        editor.IsNewReport = false;
        await editor.LoadCategoriesAsync();
        await editor.LoadReportAsync(SelectedReport.Report.ReportID);
        System.Windows.Application.Current.Dispatcher.Invoke(() => {
            var window = new System.Windows.Window
            {
                Title = "RAPOR DÜZENLEYİCİ",
                Content = new NUSHPOS.Views.ReportEditorView { DataContext = editor },
                Width = 1000,
                Height = 700,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };
            editor.RequestClose += (s, e) => window.Close();
            window.ShowDialog();
        });
    }

    private async void OnEditorRequestClose(object? sender, EventArgs e)
    {
        if (EditorViewModel != null)
        {
            EditorViewModel.RequestClose -= OnEditorRequestClose;
            EditorViewModel = null;
            IsEditingReport = false;
        }
        await InitializeReportsAsync();
    }

    private async Task InitializeReportsAsync()
    {
        using var connection = _databaseService.CreateConnection();
        
        string empIdStr = SessionManager.EmployeeID.ToString();
        
        // 1. Şifrə ilə əməkdaşın yoxlanması və məlumatların çəkilməsi
        string empQuery = @"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID, EmployeeFiles.FirstName, EmployeeFiles.LastName,  
    EmployeeFiles.SocialSecurityNumber, EmployeeFiles.SmarCardCode, EmployeeFiles.MifareCardCode, EmployeeFiles.MailingAddress, EmployeeFiles.MailingZipCode, EmployeeFiles.DateHired, EmployeeFiles.DateReleased,  
    EmployeeFiles.EmployeeActive, EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel, EmployeeFiles.AccessCode, EmployeeFiles.TipsReceived,  
    EmployeeFiles.PayBasis, EmployeeFiles.PayRate, EmployeeFiles.ScanCode, isnull(EmployeeFiles.DriverLicenseNumber,'') as DriverLicenseNumber, EmployeeFiles.DriverLicenseExpires,  
    EmployeeFiles.CarInsurancePolicyCarrier, EmployeeFiles.CarInsurancePolicyNumber, EmployeeFiles.CarInsurancePolicyExpires,  
    EmployeeFiles.CarInsurancePolicyNotes, isnull(EmployeeFiles.PrefUserInterfaceLocale,'-') as PrefUserInterfaceLocale, EmployeeFiles.EmployeeNotes, EmployeeFiles.OrderEntryUseSecLang,  
    EmployeeFiles.EmployeeIsDriver, EmployeeFiles.DefaultOEMenuGroupID, EmployeeFiles.UseStaffBank, EmployeeFiles.ScheduleNotEnforced,  
    EmployeeFiles.UseHostess, EmployeeFiles.IsAServer, EmployeeFiles.IsOffline, EmployeeFiles.NoCashierOut, EmployeeFiles.EditTimestamp,  
    EmployeeFiles.PhoneNumber, EmployeeFiles.RevenueCenterTypeID, EmployeeFiles.DeleteReason, EmployeeFiles.CustomField1, EmployeeFiles.CustomField2,  
    EmployeeFiles.CustomField3, EmployeeFiles.CustomField4, EmployeeFiles.CustomField5, EmployeeFiles.EditKey, EmployeeFiles.SyncKey,  
    EmployeeFiles.BranchID, EmployeeFiles.AddUserID, EmployeeFiles.AddDateTime, EmployeeFiles.EditUserID, EmployeeFiles.EditDateTime,  
    EmployeeTitles.TitleName as JobTitleText, isnull(EmployeeFiles.MonthlyDinnerFee,0) as MonthlyDinnerFee,CAST(0 AS BIT) AS IsChecked 
FROM EmployeeFiles  
LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  
WHERE EmployeeFiles.EmployeeID = @EmployeeID 
  AND isnull(EmployeeFiles.EmployeeActive, 0) = 1;";
        var emp = await connection.QueryFirstOrDefaultAsync(empQuery, new { EmployeeID = SessionManager.EmployeeID });

        // 2. İstifadəçiyə aid oxunmamış mesajların yoxlanması
        string msgQuery = @"
SELECT ISNULL((SELECT TOP 1 CAST(MessageKey AS NVARCHAR(50)) FROM UserMessages WHERE Reciepments LIKE '%,' + @EmpId + ',%' AND isnull(Readed,'') NOT LIKE '%,' + @EmpId + ',%'),'-');";
        var unreadMsg = await connection.ExecuteScalarAsync<string>(msgQuery, new { EmpId = empIdStr });

        // 3. 'RAPORLAR' bölməsinə giriş loqunun yazılması
        string logQuery = @"
INSERT INTO [AccessLogs] (
    [BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], [WrongPassword], [AdditionalInfo], 
    [IsSuccess], [OrderKey], [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]
) 
VALUES (
    @BranchID, GETDATE(), @StationID, @EmployeeID, N'RAPORLAR', NULL, NULL, 
    1, '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', newid(), 
    newid(), newid()
);";
        await connection.ExecuteAsync(logQuery, new { BranchID = SessionManager.BranchID, StationID = SessionManager.StationID, EmployeeID = SessionManager.EmployeeID });

        // 4. Hesabatların və kateqoriyalarının oxunması
        string reportsQuery = @"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    Reports.AutoID, Reports.ReportID, Reports.ReportKey, Reports.ReportName, Reports.ReportCategoryID, 
    Reports.ReportActive, Reports.IsPosReport, ReportCategories.CategoryName, Reports.ReportTypeID, 
    Reports.SecurityLevel, Reports.EditKey, Reports.SyncKey 
FROM Reports  
LEFT OUTER JOIN ReportCategories ON Reports.ReportCategoryID = ReportCategories.ReportCategoryID;";
        var reports = await connection.QueryAsync<ReportModel>(reportsQuery);

        // 5. Əsas hesabat kateqoriyalarının oxunması (MainCategoryID = 0)
        string catQuery = @"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [ReportCategoryID], [CategoryName], [MainCategoryID], [EditKey], [SyncKey] 
FROM [ReportCategories] 
WHERE [MainCategoryID] = 0;";
        var categories = await connection.QueryAsync<ReportCategoryModel>(catQuery);

        foreach (var category in categories)
        {
            category.Reports = new System.Collections.ObjectModel.ObservableCollection<ReportModel>(
                System.Linq.Enumerable.Where(reports, r => r.ReportCategoryID == category.ReportCategoryID)
            );
        }

        Categories = new System.Collections.ObjectModel.ObservableCollection<ReportCategoryModel>(categories);

        // 6. Hesabat və dizayn ID-lərinin sinxronizasiyası və defolt dizaynların tənzimlənməsi
        string updateQuery = @"
UPDATE Reports SET ReportID = AutoID;
UPDATE ReportDesigns SET ReportDesignID = AutoID;
UPDATE ReportDesigns SET ReportID = Reports.AutoID 
FROM ReportDesigns 
INNER JOIN Reports ON ReportDesigns.ReportKey = Reports.ReportKey;
UPDATE ReportDesigns SET IsDefault = 1 
WHERE AutoID IN (
    SELECT min(d.AutoID) 
    FROM ReportDesigns AS d 
    LEFT OUTER JOIN ReportDesigns AS rd ON rd.ReportKey = d.ReportKey AND rd.IsDefault = 1 
    WHERE isnull(d.IsDefault, 0) = 0 AND rd.AutoID IS NULL 
    GROUP BY d.ReportID
);";
        await connection.ExecuteAsync(updateQuery);
    }

    [RelayCommand]
    private void GoBack()
    {
        // Geri qayıtmaq üçün MainScreenViewModel-ə və ya OperationsViewModel-ə keçid
        _navigationService.NavigateTo<OperationsViewModel>(); 
    }

    [RelayCommand]
    private void OpenReport(ReportModel report)
    {
        if (report == null) return;
        var existingTab = System.Linq.Enumerable.FirstOrDefault(OpenReports, t => t.Report.ReportID == report.ReportID);
        if (existingTab != null)
        {
            SelectedReport = existingTab;
        }
        else
        {
            var newTab = new ReportTabModel(report, _databaseService);
            OpenReports.Add(newTab);
            SelectedReport = newTab;
            _ = newTab.ApplyAsync();
        }
    }

    [RelayCommand]
    private void CloseReport(ReportTabModel tab)
    {
        if (tab != null && OpenReports.Contains(tab))
        {
            OpenReports.Remove(tab);
        }
    }
}

public class ReportCategoryModel
{
    public int ReportCategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int MainCategoryID { get; set; }
    public System.Collections.ObjectModel.ObservableCollection<ReportModel> Reports { get; set; } = new();
}

public class ReportModel
{
    public int AutoID { get; set; }
    public int ReportID { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public int ReportCategoryID { get; set; }
    public int ReportTypeID { get; set; }
}

public class QueryResult
{
    public string QueryName { get; set; } = string.Empty;
    public System.Data.DataTable Data { get; set; } = new();
    public System.Data.DataView DataView => Data.DefaultView;
}

public partial class ReportTabModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public ReportModel Report { get; }

    public bool IsKasiyerCikis => Report.ReportID == 35;
    public bool IsGenericReport => Report.ReportID != 35;
    
    [ObservableProperty]
    private System.DateTime _startDate = System.DateTime.Today.AddHours(6); // 06:00:00
    [ObservableProperty]
    private System.DateTime _endDate = System.DateTime.Today.AddDays(1).AddHours(5).AddMinutes(59).AddSeconds(59);

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<QueryResult> _queryResults = new();

    [ObservableProperty]
    private string _storeName = "PRIVE STEAK GALLERY";
    
    [ObservableProperty]
    private string _reportDateTimeText = "";

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<dynamic> _categorySales = new();
    
    [ObservableProperty]
    private decimal _grossSales;
    [ObservableProperty]
    private decimal _checkDiscounts;
    [ObservableProperty]
    private decimal _itemDiscounts;
    [ObservableProperty]
    private decimal _cashDiscounts;
    [ObservableProperty]
    private decimal _netSales;
    [ObservableProperty]
    private decimal _serviceFee;

    [ObservableProperty]
    private int _totalCheckCount;
    [ObservableProperty]
    private decimal _checkAverage;
    [ObservableProperty]
    private int _totalGuests;
    [ObservableProperty]
    private decimal _guestAverage;

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<dynamic> _salesTypes = new();

    [ObservableProperty]
    private string _htmlContent = string.Empty;

    public bool IsHtmlReport => Report.ReportTypeID == 1;

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<dynamic> _payments = new();

    [ObservableProperty]
    private decimal _openChecksAmount;

    public ReportTabModel(ReportModel report, DatabaseService databaseService)
    {
        Report = report;
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task ApplyAsync()
    {
        ReportDateTimeText = $"{StartDate:dd.MM.yyyy HH:mm}  {EndDate:dd.MM.yyyy HH:mm}\nSaat: {System.DateTime.Now:HH:mm:ss}";
        
        using var conn = _databaseService.CreateConnection();
        string getQueriesSql = @"
            SELECT QueryName, 
                REPLACE(
                REPLACE(
                REPLACE(
                REPLACE( 
                REPLACE( 
                CAST([QueryData] AS NVARCHAR(MAX)), 
                'GlobalPaymentMethods','PaymentMethods'),
                'GlobalEmployeeFiles','EmployeeFiles'),
                'GlobalMenuItems','MenuItems'),
                'GlobalPromotions','Promotions'),
                'GlobalDiscounts','Discounts') AS QueryData
            FROM ReportQueries WHERE ReportID = @ReportID";
        var queries = await conn.QueryAsync(getQueriesSql, new { ReportID = Report.ReportID });
        
        System.Windows.Application.Current.Dispatcher.Invoke(() => {
            QueryResults.Clear();
        });

        foreach (var q in queries)
        {
            string qName = q.QueryName;
            string qData = q.QueryData;

            try
            {
                // Generate generic DataTable for any report
                var dt = new System.Data.DataTable();
                using (var reader = await conn.ExecuteReaderAsync(qData, new { date1 = StartDate, date2 = EndDate }))
                {
                    dt.Load(reader);
                }
                
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    QueryResults.Add(new QueryResult { QueryName = qName, Data = dt });
                });

                if (Report.ReportID == 35)
                {
                    if (qName == "GRUPSATISLAR")
                    {
                        var list = new System.Collections.ObjectModel.ObservableCollection<dynamic>();
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            list.Add(new { 
                                GroupName = dt.Columns.Contains("Grup") ? row["Grup"] : "", 
                                Percentage = dt.Columns.Contains("Yuzde") ? row["Yuzde"] : 0m, 
                                Amount = dt.Columns.Contains("tutar") ? row["tutar"] : 0m 
                            });
                        }
                        CategorySales = list;
                    }
                    else if (qName == "SATISTIPLERI")
                    {
                        var list = new System.Collections.ObjectModel.ObservableCollection<dynamic>();
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            object amount = dt.Columns.Count > 0 ? row[0] : 0m;
                            list.Add(new { 
                                SalesType = dt.Columns.Contains("Satış Türü") ? row["Satış Türü"] : "", 
                                Count = dt.Columns.Contains("adet") ? row["adet"] : 0, 
                                Amount = amount 
                            });
                        }
                        SalesTypes = list;
                    }
                    else if (qName == "ISLETMEBILGILERI")
                    {
                        if (dt.Rows.Count > 0 && dt.Columns.Contains("ADI"))
                            StoreName = dt.Rows[0]["ADI"].ToString();
                    }
                    else if (qName == "INDIRIMLER")
                    {
                        if (dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            GrossSales = dt.Columns.Contains("Brüt Satışlar") && row["Brüt Satışlar"] != DBNull.Value ? Convert.ToDecimal(row["Brüt Satışlar"]) : 0m;
                            ItemDiscounts = dt.Columns.Contains("Ürün İndirimleri") && row["Ürün İndirimleri"] != DBNull.Value ? Convert.ToDecimal(row["Ürün İndirimleri"]) : 0m;
                            CashDiscounts = dt.Columns.Contains("Nakit İndirimler") && row["Nakit İndirimler"] != DBNull.Value ? Convert.ToDecimal(row["Nakit İndirimler"]) : 0m;
                            CheckDiscounts = dt.Columns.Contains("Çek İndirimler") && row["Çek İndirimler"] != DBNull.Value ? Convert.ToDecimal(row["Çek İndirimler"]) : 0m;
                            NetSales = dt.Columns.Contains("Net Satışlar") && row["Net Satışlar"] != DBNull.Value ? Convert.ToDecimal(row["Net Satışlar"]) : 0m;
                            TotalCheckCount = dt.Columns.Contains("Toplam Çek Sayısı") && row["Toplam Çek Sayısı"] != DBNull.Value ? Convert.ToInt32(row["Toplam Çek Sayısı"]) : 0;
                            TotalGuests = dt.Columns.Contains("Toplam Konuk") && row["Toplam Konuk"] != DBNull.Value ? Convert.ToInt32(row["Toplam Konuk"]) : 0;
                            GuestAverage = dt.Columns.Contains("Kişi Başı Ortalama") && row["Kişi Başı Ortalama"] != DBNull.Value ? Convert.ToDecimal(row["Kişi Başı Ortalama"]) : 0m;
                            CheckAverage = dt.Columns.Contains("Çek Ortalama") && row["Çek Ortalama"] != DBNull.Value ? Convert.ToDecimal(row["Çek Ortalama"]) : 0m;
                        }
                    }
                    else if (qName == "ACIKCEKLERR")
                    {
                        if (dt.Rows.Count > 0 && dt.Columns.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                            OpenChecksAmount = Convert.ToDecimal(dt.Rows[0][0]);
                    }
                    else if (qName == "KAZANCDURUMUNET")
                    {
                        var list = new System.Collections.ObjectModel.ObservableCollection<dynamic>();
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            object amount = dt.Columns.Contains("GenelToplam") ? row["GenelToplam"] : (dt.Columns.Count > 1 ? row[1] : 0m);
                            list.Add(new { 
                                PaymentName = dt.Columns.Contains("PaymentMethodName") ? row["PaymentMethodName"] : (dt.Columns.Count > 0 ? row[0] : ""), 
                                Amount = amount 
                            });
                        }
                        Payments = list;
                    }
                    else if (qName == "ServisBedeli")
                    {
                        if (dt.Rows.Count > 0 && dt.Columns.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                            ServiceFee = Convert.ToDecimal(dt.Rows[0][0]);
                    }
                }
            }
            catch
            {
                // Ignore query execution errors for now
            }
        }

        if (Report.ReportTypeID == 1)
        {
            string designHtml = "";
            var designBytes = await conn.QueryFirstOrDefaultAsync<byte[]>("SELECT DesignData FROM ReportDesigns WHERE ReportID = @ReportID", new { ReportID = Report.ReportID });
            if (designBytes != null && designBytes.Length > 0)
            {
                try {
                    string decoded = System.Text.Encoding.UTF8.GetString(designBytes);
                    if (!decoded.StartsWith("/// <XRTypeInfo>"))
                        designHtml = decoded;
                } catch { }
            }
            
            if (!string.IsNullOrWhiteSpace(designHtml))
            {
                foreach (var res in QueryResults)
                {
                    string placeholder = "{{" + res.QueryName + "}}";
                    if (designHtml.Contains(placeholder))
                    {
                        string htmlTable = GenerateHtmlTable(res.Data);
                        designHtml = designHtml.Replace(placeholder, htmlTable);
                    }
                }
                
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    HtmlContent = designHtml;
                });
            }
        }
    }

    private string GenerateHtmlTable(System.Data.DataTable dt)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<table class='report-table'>");
        sb.AppendLine("<thead><tr>");
        
        foreach (System.Data.DataColumn col in dt.Columns)
        {
            string colName = col.ColumnName;
            string align = (col.DataType == typeof(decimal) || col.DataType == typeof(double) || col.DataType == typeof(int)) ? "right" : "left";
            sb.AppendLine($"<th class='text-{align}'>{colName}</th>");
        }
        sb.AppendLine("</tr></thead>");
        
        sb.AppendLine("<tbody>");
        foreach (System.Data.DataRow row in dt.Rows)
        {
            sb.AppendLine("<tr>");
            foreach (System.Data.DataColumn col in dt.Columns)
            {
                string val = row[col] != DBNull.Value ? row[col].ToString()! : "";
                if (col.DataType == typeof(decimal) || col.DataType == typeof(double))
                {
                    if (decimal.TryParse(val, out decimal decVal))
                        val = decVal.ToString("N2");
                }
                string align = (col.DataType == typeof(decimal) || col.DataType == typeof(double) || col.DataType == typeof(int)) ? "right" : "left";
                sb.AppendLine($"<td class='text-{align}'>{val}</td>");
            }
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</tbody></table>");
        return sb.ToString();
    }

    [RelayCommand]
    private void ExportToExcel()
    {
        try
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"{Report.ReportName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    foreach (var res in QueryResults)
                    {
                        if (res.Data != null && res.Data.Columns.Count > 0)
                        {
                            var sheetName = res.QueryName;
                            if (string.IsNullOrWhiteSpace(sheetName)) sheetName = "Səhifə";
                            if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);
                            foreach(var c in new[] {'*', ':', '?', '/', '\\', '[', ']'})
                                sheetName = sheetName.Replace(c, '_');

                            string finalSheetName = sheetName;
                            int counter = 1;
                            while (workbook.Worksheets.TryGetWorksheet(finalSheetName, out _))
                            {
                                string suffix = $"_{counter}";
                                finalSheetName = sheetName.Length + suffix.Length > 31 
                                    ? $"{sheetName.Substring(0, 31 - suffix.Length)}{suffix}"
                                    : $"{sheetName}{suffix}";
                                counter++;
                            }

                            var worksheet = workbook.Worksheets.Add(finalSheetName);
                            var table = worksheet.Cell(1, 1).InsertTable(res.Data);
                            worksheet.Columns().AdjustToContents();
                        }
                    }
                    
                    if (workbook.Worksheets.Count == 0)
                    {
                        workbook.Worksheets.Add("Boş").Cell(1,1).Value = "Məlumat yoxdur";
                    }
                    
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                
                System.Windows.MessageBox.Show("Hesabat Excel-ə uğurla ixrac edildi!", "Uğurlu", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                
                try
                {
                    Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Excel-ə ixrac zamanı xəta baş verdi:\n{ex.Message}", "Xəta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}
