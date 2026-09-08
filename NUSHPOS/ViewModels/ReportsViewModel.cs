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
using System.Runtime.InteropServices;

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

    public ReportsViewModel(NavigationService navigationService, DatabaseService databaseService, IServiceProvider serviceProvider, StationSettingsService stationSettingsService)
    {
        _navigationService = navigationService;
        _databaseService = databaseService;
        _serviceProvider = serviceProvider;
        _stationSettingsService = stationSettingsService;
        Title = "Raporlar";
        _ = InitializeReportsAsync();
    }

    private readonly StationSettingsService _stationSettingsService;

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
            var newTab = new ReportTabModel(report, _databaseService, _stationSettingsService);
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

    private readonly StationSettingsService _stationSettingsService;

    public ReportTabModel(ReportModel report, DatabaseService databaseService, StationSettingsService stationSettingsService)
    {
        Report = report;
        _databaseService = databaseService;
        _stationSettingsService = stationSettingsService;
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
                // 1. Translate section titles in design HTML
                designHtml = System.Text.RegularExpressions.Regex.Replace(designHtml, @"<div class=""section-title"">\s*([^<]+)\s*</div>", m =>
                {
                    string rawTitle = m.Groups[1].Value.Trim();
                    if (rawTitle.Equals("ISLETMEBILGILERI", StringComparison.OrdinalIgnoreCase)) return "";
                    string trTitle = TranslateSectionTitle(rawTitle);
                    return $@"<div class=""section-title"">{trTitle}</div>";
                }, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                foreach (var res in QueryResults)
                {
                    string placeholder = "{{" + res.QueryName + "}}";
                    if (designHtml.Contains(placeholder))
                    {
                        string htmlTable = GenerateHtmlTable(res.QueryName, res.Data);
                        if (string.IsNullOrWhiteSpace(htmlTable))
                        {
                            string titlePattern = @"<div class=""section-title"">\s*[^<]*\s*</div>\s*" + System.Text.RegularExpressions.Regex.Escape(placeholder);
                            designHtml = System.Text.RegularExpressions.Regex.Replace(designHtml, titlePattern, "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                            string sectionPattern = @"<div class=""section"">\s*<div class=""section-title"">\s*[^<]*\s*</div>\s*" + System.Text.RegularExpressions.Regex.Escape(placeholder) + @"\s*</div>";
                            designHtml = System.Text.RegularExpressions.Regex.Replace(designHtml, sectionPattern, "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                            designHtml = designHtml.Replace(placeholder, "");
                        }
                        else
                        {
                            designHtml = designHtml.Replace(placeholder, htmlTable);
                        }
                    }
                }
                
                designHtml = System.Text.RegularExpressions.Regex.Replace(designHtml, @"\{\{[^}]+\}\}", "");

                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    HtmlContent = designHtml;
                });
            }
        }
    }

    private static string TranslateColumnName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        string lower = name.Trim().ToLowerInvariant();
        return lower switch
        {
            "grup" => "Qrup",
            "tutar" or "tutari" or "geneltoplam" or "toplam" or "column1" or "amount" or "grossamount" => "Məbləğ",
            "yuzde" or "oran" => "Faiz",
            "adet" or "say" or "count" or "miktar" => "Say",
            "satış türü" or "satisturu" or "salestype" => "Satış Növü",
            "paymentmethodname" or "odemenovu" or "odemeturu" or "ödeme yöntemi" or "odeme" => "Ödəniş Üsulu",
            "amountpaid" or "ödenen" or "odenen" or "netsales" => "Ümumi Məbləğ",
            "servis bedeli" or "servisbedeli" or "servicefee" => "Xidmət Haqqı",
            "cekno" or "çek no" or "receiptno" => "Çek №",
            "workdate" => "Tarix",
            "clockin" => "Giriş",
            "clockout" => "Çıxış",
            "firstname" => "Ad",
            "lastname" => "Soyad",
            "menuitemtext" => "Məhsul",
            "quantity" => "Say",
            "unitprice" => "Qiymət",
            "extendedprice" => "Məbləğ",
            _ => name
        };
    }

    private static string TranslateCellValue(string val)
    {
        if (string.IsNullOrWhiteSpace(val)) return "";
        string trimmed = val.Trim();
        if (trimmed.Equals("MUTFAK", StringComparison.OrdinalIgnoreCase)) return "MƏTBƏX";
        if (trimmed.Equals("MASA SERVIS", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("MASA SERVISI", StringComparison.OrdinalIgnoreCase)) return "MASA XİDMƏTİ";
        if (trimmed.Equals("PAKET SERVIS", StringComparison.OrdinalIgnoreCase)) return "PAKET XİDMƏTİ";
        if (trimmed.Equals("GEL AL", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("GEL-AL", StringComparison.OrdinalIgnoreCase)) return "GƏL-AL";
        return trimmed;
    }

    private static string TranslateSectionTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return "";
        string lower = title.Trim().ToLowerInvariant();
        return lower switch
        {
            "grupsatislar" => "SATIŞ XÜLASƏSİ",
            "indirimler" => "ENDİRİMLƏR VƏ STATİSTİKA",
            "servisbedeli" => "XİDMƏT HAQQI",
            "satistipleri" => "SATIŞ PAYLANMASI",
            "kazancdurumunet" => "SATIŞ GƏLİRLƏRİ",
            "acikceklerr" or "acikcekler" => "AÇIQ ÇEKLƏR",
            "qaytarmalar" => "QAYTARMALAR (İADƏ)",
            "legv" or "iptaller" => "LƏĞV ƏMƏLİYYATLARI",
            "carihesab" => "CARİ HESAB ƏMƏLİYYATLARI",
            "isci_giris_cixis" => "İŞÇİ GİRİŞ-ÇIXIŞI",
            _ => title
        };
    }

    private string GenerateHtmlTable(string queryName, System.Data.DataTable dt)
    {
        if (dt == null || dt.Rows.Count == 0) return "";

        if (queryName.Equals("ISLETMEBILGILERI", StringComparison.OrdinalIgnoreCase))
        {
            return ""; // Omit table for business info
        }

        if (queryName.Equals("INDIRIMLER", StringComparison.OrdinalIgnoreCase))
        {
            var sbInd = new System.Text.StringBuilder();
            sbInd.AppendLine("<table class='kv-table' style='width:100%'>");
            sbInd.AppendLine($"<tr><td class='kv-label'>Ümumi Satış:</td><td class='kv-value'>{GrossSales:N2}</td></tr>");
            sbInd.AppendLine($"<tr><td class='kv-label'>Xalis Satış:</td><td class='kv-value'>{NetSales:N2}</td></tr>");
            if (CheckDiscounts > 0)
                sbInd.AppendLine($"<tr><td class='kv-label'>Çek Endirimləri:</td><td class='kv-value'>{CheckDiscounts:N2}</td></tr>");
            if (ItemDiscounts > 0)
                sbInd.AppendLine($"<tr><td class='kv-label'>Məhsul Endirimləri:</td><td class='kv-value'>{ItemDiscounts:N2}</td></tr>");
            if (CashDiscounts > 0)
                sbInd.AppendLine($"<tr><td class='kv-label'>Nağd Endirimlər:</td><td class='kv-value'>{CashDiscounts:N2}</td></tr>");
            sbInd.AppendLine($"<tr><td class='kv-label'>Çek Sayı:</td><td class='kv-value'>{TotalCheckCount}</td></tr>");
            sbInd.AppendLine($"<tr><td class='kv-label'>Qonaq Sayı:</td><td class='kv-value'>{TotalGuests}</td></tr>");
            sbInd.AppendLine($"<tr><td class='kv-label'>Adam Başı Orta:</td><td class='kv-value'>{GuestAverage:N2}</td></tr>");
            sbInd.AppendLine($"<tr><td class='kv-label'>Orta Çek:</td><td class='kv-value'>{CheckAverage:N2}</td></tr>");
            sbInd.AppendLine("</table>");
            return sbInd.ToString();
        }

        if (queryName.Equals("ServisBedeli", StringComparison.OrdinalIgnoreCase))
        {
            var sbSrv = new System.Text.StringBuilder();
            sbSrv.AppendLine("<table class='kv-table' style='width:100%'>");
            sbSrv.AppendLine($"<tr><td class='kv-label'>Xidmət Haqqı:</td><td class='kv-value'>{ServiceFee:N2}</td></tr>");
            sbSrv.AppendLine("</table>");
            return sbSrv.ToString();
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<table class='report-table'>");
        sb.AppendLine("<thead><tr>");
        
        foreach (System.Data.DataColumn col in dt.Columns)
        {
            string colName = TranslateColumnName(col.ColumnName);
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
                else
                {
                    val = TranslateCellValue(val);
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

    public System.Windows.Controls.WebBrowser? ActiveWebBrowser { get; set; }

    [RelayCommand]
    private async Task PrintPosReportAsync()
    {
        try
        {
            var printerSettings = await _stationSettingsService.GetStationPrinterSettingsAsync(SessionManager.StationID);
            string printerName = printerSettings?.ReportPrinterName ?? printerSettings?.CheckPrinterName ?? "";
            
            if (string.IsNullOrWhiteSpace(printerName) || printerName == "-")
            {
                // If POS report printer not configured, ask user to choose
                await PrintPromptPrinterAsync();
                return;
            }

            await PrintToPrinterAsync(printerName);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Hesabat çap edilərkən xəta baş verdi:\n{ex.Message}", "Xəta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task PrintA4ReportAsync()
    {
        try
        {
            var printerSettings = await _stationSettingsService.GetStationPrinterSettingsAsync(SessionManager.StationID);
            string printerName = printerSettings?.ReportA4PrinterName ?? "";
            
            if (string.IsNullOrWhiteSpace(printerName) || printerName == "-")
            {
                // If A4 report printer not configured, ask user to choose
                await PrintPromptPrinterAsync();
                return;
            }

            await PrintToPrinterAsync(printerName);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Hesabat çap edilərkən xəta baş verdi:\n{ex.Message}", "Xəta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task PrintPromptPrinterAsync()
    {
        if (IsHtmlReport && ActiveWebBrowser != null)
        {
            bool printed = false;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (ActiveWebBrowser.Document != null)
                    {
                        dynamic doc = ActiveWebBrowser.Document;
                        doc.execCommand("Print", true, null);
                        printed = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WebBrowser print dialog error: {ex.Message}");
                }
            });

            if (printed) return;
        }

        string? selectedPrinter = null;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            using var dlg = new System.Windows.Forms.PrintDialog();
            dlg.UseEXDialog = true;
            dlg.AllowSomePages = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPrinter = dlg.PrinterSettings.PrinterName;
            }
        });

        if (string.IsNullOrWhiteSpace(selectedPrinter)) return;

        await PrintToPrinterAsync(selectedPrinter);
    }

    [ComImport]
    [Guid("0000010d-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IViewObject
    {
        [PreserveSig]
        int Draw(
            [In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect,
            int lindex,
            IntPtr pvAspect,
            [In] IntPtr ptd,
            IntPtr hdcTargetDev,
            IntPtr hdcDraw,
            [In] ref RECT lprcBounds,
            [In] IntPtr lprcWBounds,
            IntPtr pfnContinue,
            [In] IntPtr dwContinue);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    private static void BinarizeBitmap(System.Drawing.Bitmap bmp, int threshold = 210)
    {
        var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
        var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        try
        {
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

            int bytesPerPixel = 4;
            for (int y = 0; y < bmp.Height; y++)
            {
                int rowStart = y * bmpData.Stride;
                for (int x = 0; x < bmp.Width; x++)
                {
                    int idx = rowStart + (x * bytesPerPixel);
                    if (idx + 3 >= bytes) continue;

                    byte b = rgbValues[idx];
                    byte g = rgbValues[idx + 1];
                    byte r = rgbValues[idx + 2];

                    int gray = (r * 30 + g * 59 + b * 11) / 100;

                    if (gray < threshold)
                    {
                        // Solid Pure Black
                        rgbValues[idx] = 0;
                        rgbValues[idx + 1] = 0;
                        rgbValues[idx + 2] = 0;
                        rgbValues[idx + 3] = 255;
                    }
                    else
                    {
                        // Pure White
                        rgbValues[idx] = 255;
                        rgbValues[idx + 1] = 255;
                        rgbValues[idx + 2] = 255;
                        rgbValues[idx + 3] = 255;
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpData.Scan0, bytes);
        }
        finally
        {
            bmp.UnlockBits(bmpData);
        }
    }

    private System.Drawing.Bitmap? RenderWebBrowserToBitmap()
    {
        if (ActiveWebBrowser == null) return null;

        System.Drawing.Bitmap? resultBmp = null;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                if (ActiveWebBrowser.Document == null) return;
                dynamic doc = ActiveWebBrowser.Document;
                dynamic body = doc.body;
                if (body == null) return;

                // Save original zoom
                string originalZoom = "";
                try { originalZoom = body.style.zoom ?? "100%"; } catch { }

                // Set zoom to 2.0 (200%) for high DPI / crystal clear text
                try { body.style.zoom = "2.0"; } catch { }

                int docWidth = 0;
                int docHeight = 0;
                try { docWidth = (int)body.scrollWidth; } catch { }
                try { docHeight = (int)body.scrollHeight; } catch { }

                if (docWidth <= 50) docWidth = 600;
                if (docHeight <= 50) docHeight = 2000;

                using var tempBmp = new System.Drawing.Bitmap(docWidth, docHeight);
                using (var g = System.Drawing.Graphics.FromImage(tempBmp))
                {
                    g.Clear(System.Drawing.Color.White);
                    IntPtr hdc = g.GetHdc();
                    try
                    {
                        if (ActiveWebBrowser.Document is IViewObject viewObject)
                        {
                            var rect = new RECT { left = 0, top = 0, right = docWidth, bottom = docHeight };
                            viewObject.Draw(1 /* DVASPECT_CONTENT */, -1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, hdc, ref rect, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                    finally
                    {
                        g.ReleaseHdc(hdc);
                    }
                }

                // Restore original zoom
                try { body.style.zoom = originalZoom; } catch { }

                try
                {
                    dynamic container = doc.querySelector(".receipt-container") ?? doc.querySelector(".page-container");
                    if (container != null)
                    {
                        int cLeft = (int)container.offsetLeft;
                        int cTop = (int)container.offsetTop;
                        int cWidth = (int)container.offsetWidth;
                        int cHeight = (int)container.offsetHeight;

                        if (cWidth > 100 && cHeight > 100 && cLeft >= 0 && cTop >= 0 && cLeft + cWidth <= docWidth && cTop + cHeight <= docHeight)
                        {
                            var cropRect = new System.Drawing.Rectangle(cLeft, cTop, cWidth, cHeight);
                            resultBmp = tempBmp.Clone(cropRect, tempBmp.PixelFormat);
                        }
                    }
                }
                catch { }

                if (resultBmp == null)
                {
                    resultBmp = (System.Drawing.Bitmap)tempBmp.Clone();
                }

                if (resultBmp != null)
                {
                    BinarizeBitmap(resultBmp, 210);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RenderWebBrowserToBitmap error: {ex.Message}");
            }
        });

        return resultBmp;
    }

    public async Task PrintToPrinterAsync(string printerName)
    {
        if (string.IsNullOrWhiteSpace(printerName)) return;

        await Task.Run(() =>
        {
            try
            {
                using var printDoc = new System.Drawing.Printing.PrintDocument();
                printDoc.PrinterSettings.PrinterName = printerName;
                printDoc.PrintController = new System.Drawing.Printing.StandardPrintController();
                printDoc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);

                var printableLines = GetPrintableLines();
                if (printableLines.Count == 0) return;

                int lineIndex = 0;
                int pageCount = 0;
                const int maxPages = 5;

                printDoc.PrintPage += (s, e) =>
                {
                    if (e.Graphics == null) return;
                    pageCount++;

                    float pageWidth = e.PageBounds.Width > 0 ? e.PageBounds.Width : 283;
                    float targetWidth = (int)(pageWidth * 0.90f);
                    float y = 10;
                    float leftMargin = 5;

                    using var fontRegular = new System.Drawing.Font("Consolas", 8.5f, System.Drawing.FontStyle.Regular);
                    using var fontBold = new System.Drawing.Font("Consolas", 8.5f, System.Drawing.FontStyle.Bold);
                    using var fontTitle = new System.Drawing.Font("Consolas", 10.0f, System.Drawing.FontStyle.Bold);
                    using var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

                    float lineHeight = fontRegular.GetHeight(e.Graphics) + 2;

                    while (lineIndex < printableLines.Count)
                    {
                        var line = printableLines[lineIndex];

                        if (y + lineHeight > e.PageBounds.Height - 20 && e.PageBounds.Height > 100)
                        {
                            if (pageCount < maxPages)
                            {
                                e.HasMorePages = true;
                                return;
                            }
                            break;
                        }

                        var font = line.IsTitle ? fontTitle : (line.IsBold ? fontBold : fontRegular);

                        if (line.IsCenter)
                        {
                            var size = e.Graphics.MeasureString(line.Text, font);
                            float x = Math.Max(leftMargin, (targetWidth - size.Width) / 2);
                            e.Graphics.DrawString(line.Text, font, brush, x, y);
                        }
                        else
                        {
                            e.Graphics.DrawString(line.Text, font, brush, leftMargin, y);
                        }

                        y += lineHeight;
                        lineIndex++;
                    }

                    e.HasMorePages = (lineIndex < printableLines.Count && pageCount < maxPages);
                };

                printDoc.Print();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Report Print Error: {ex.Message}");
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.MessageBox.Show($"Hesabat çap edilərkən xəta baş verdi:\n{ex.Message}", "Çap Xətası", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                });
            }
        });
    }

    public class PrintableLine
    {
        public string Text { get; set; } = string.Empty;
        public bool IsBold { get; set; }
        public bool IsTitle { get; set; }
        public bool IsCenter { get; set; }
    }

    private List<PrintableLine> GetPrintableLines()
    {
        var list = new List<PrintableLine>();

        if (!string.IsNullOrWhiteSpace(HtmlContent))
        {
            var raw = HtmlContent;
            // 1. Strip scripts, styles, comments
            raw = System.Text.RegularExpressions.Regex.Replace(raw, @"<script[\s\S]*?</script>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            raw = System.Text.RegularExpressions.Regex.Replace(raw, @"<style[\s\S]*?</style>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            raw = System.Text.RegularExpressions.Regex.Replace(raw, @"<!--[\s\S]*?-->", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 2. Process all <table> elements
            raw = System.Text.RegularExpressions.Regex.Replace(raw, @"<table[^>]*>([\s\S]*?)</table>", m =>
            {
                string tableContent = m.Groups[1].Value;
                var rows = System.Text.RegularExpressions.Regex.Matches(tableContent, @"<tr[^>]*>([\s\S]*?)</tr>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                
                var tbodyMatch = System.Text.RegularExpressions.Regex.Match(tableContent, @"<tbody[^>]*>([\s\S]*?)</tbody>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (tbodyMatch.Success)
                {
                    var bodyRows = System.Text.RegularExpressions.Regex.Matches(tbodyMatch.Groups[1].Value, @"<tr[^>]*>([\s\S]*?)</tr>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (bodyRows.Count == 0) return "";
                }
                else if (rows.Count <= 1)
                {
                    return "";
                }

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("\n----------------------------------------");

                bool firstRow = true;
                foreach (System.Text.RegularExpressions.Match row in rows)
                {
                    var cells = System.Text.RegularExpressions.Regex.Matches(row.Groups[1].Value, @"<(td|th)[^>]*>([\s\S]*?)</\1>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (cells.Count > 0)
                    {
                        var cellTexts = new List<string>();
                        foreach (System.Text.RegularExpressions.Match c in cells)
                        {
                            string t = System.Text.RegularExpressions.Regex.Replace(c.Groups[2].Value, "<.*?>", "").Trim();
                            t = System.Net.WebUtility.HtmlDecode(t);
                            cellTexts.Add(t);
                        }
                        string formatted = FormatTableRow(cellTexts, 40);
                        sb.AppendLine(formatted);

                        if (firstRow)
                        {
                            sb.AppendLine("----------------------------------------");
                            firstRow = false;
                        }
                    }
                }

                sb.AppendLine("----------------------------------------\n");
                return sb.ToString();
            }, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 3. Format headers and sections
            raw = System.Text.RegularExpressions.Regex.Replace(raw, @"<div class=""section-title""[^>]*>([\s\S]*?)</div>", "\n----------------------------------------\n__TITLE__$1\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            raw = System.Text.RegularExpressions.Regex.Replace(raw, @"<div class=""report-header""[^>]*>([\s\S]*?)</div>", "__HEADER__$1\n----------------------------------------\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            raw = raw.Replace("<hr>", "\n----------------------------------------\n")
                     .Replace("<hr/>", "\n----------------------------------------\n")
                     .Replace("<hr />", "\n----------------------------------------\n")
                     .Replace("<br>", "\n")
                     .Replace("<br/>", "\n")
                     .Replace("<br />", "\n")
                     .Replace("</div>", "\n")
                     .Replace("</p>", "\n");

            // 4. Clean remaining HTML tags
            raw = System.Text.RegularExpressions.Regex.Replace(raw, "<.*?>", "");
            raw = System.Net.WebUtility.HtmlDecode(raw);

            var lines = raw.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            bool inHeader = false;
            foreach (var l in lines)
            {
                string trimmed = l.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                if (trimmed.StartsWith("__HEADER__"))
                {
                    trimmed = trimmed.Substring(10).Trim();
                    inHeader = true;
                }
                if (trimmed.StartsWith("---"))
                {
                    inHeader = false;
                }

                bool isTitle = trimmed.StartsWith("__TITLE__") || trimmed.StartsWith("**") || trimmed.Equals("SATIŞ XÜLASƏSİ", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("KASİYER ÇIXIŞI", StringComparison.OrdinalIgnoreCase);
                if (trimmed.StartsWith("__TITLE__")) trimmed = trimmed.Substring(9).Trim();

                bool isCenter = inHeader || isTitle || trimmed.StartsWith("---") || trimmed.StartsWith("===") || trimmed.StartsWith("07.") || trimmed.StartsWith("Saat:") || trimmed.Equals("BAKU YASAMAL", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("NUSHPOS", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("ADI", StringComparison.OrdinalIgnoreCase);
                bool isBold = inHeader || isTitle || trimmed.StartsWith("TOPLAM") || trimmed.StartsWith("ÜMUMİ") || trimmed.StartsWith("XALİS") || trimmed.Contains("Satış:");

                // Format key-value pairs cleanly (e.g. "Ümumi Satış:          1.451,28")
                if (trimmed.Contains(":") && !trimmed.StartsWith("Saat:") && !trimmed.StartsWith("http") && !trimmed.StartsWith("07."))
                {
                    int colonIdx = trimmed.IndexOf(':');
                    string key = trimmed.Substring(0, colonIdx + 1).Trim();
                    string val = trimmed.Substring(colonIdx + 1).Trim();
                    if (!string.IsNullOrWhiteSpace(val) && val.Length < 15 && key.Length < 25)
                    {
                        int space = 40 - key.Length - val.Length;
                        if (space > 0)
                        {
                            trimmed = key + new string(' ', space) + val;
                        }
                    }
                }

                list.Add(new PrintableLine
                {
                    Text = trimmed,
                    IsBold = isBold,
                    IsTitle = isTitle,
                    IsCenter = isCenter
                });
            }
        }
        else if (QueryResults.Count > 0)
        {
            list.Add(new PrintableLine { Text = Report.ReportName, IsTitle = true, IsCenter = true });
            list.Add(new PrintableLine { Text = ReportDateTimeText, IsCenter = true });
            list.Add(new PrintableLine { Text = "----------------------------------------", IsCenter = true });

            foreach (var q in QueryResults)
            {
                if (!string.IsNullOrWhiteSpace(q.QueryName))
                {
                    list.Add(new PrintableLine { Text = q.QueryName, IsBold = true });
                    list.Add(new PrintableLine { Text = "----------------------------------------" });
                }

                if (q.Data != null && q.Data.Rows.Count > 0)
                {
                    var headers = new List<string>();
                    foreach (System.Data.DataColumn col in q.Data.Columns) headers.Add(col.ColumnName);
                    list.Add(new PrintableLine { Text = FormatTableRow(headers, 40), IsBold = true });

                    foreach (System.Data.DataRow row in q.Data.Rows)
                    {
                        var rowVals = new List<string>();
                        foreach (System.Data.DataColumn col in q.Data.Columns)
                        {
                            string v = row[col] != DBNull.Value ? row[col].ToString()! : "";
                            if (col.DataType == typeof(decimal) || col.DataType == typeof(double))
                            {
                                if (decimal.TryParse(v, out decimal dec)) v = dec.ToString("N2");
                            }
                            rowVals.Add(v);
                        }
                        list.Add(new PrintableLine { Text = FormatTableRow(rowVals, 40) });
                    }
                    list.Add(new PrintableLine { Text = "----------------------------------------" });
                }
            }
        }

        return list;
    }

    private static string FormatTableRow(List<string> cells, int totalWidth)
    {
        if (cells.Count == 0) return "";
        if (cells.Count == 1) return cells[0];
        if (cells.Count == 2)
        {
            int col1Width = totalWidth - 14;
            string c1 = cells[0].Length > col1Width ? cells[0].Substring(0, col1Width) : cells[0];
            return c1.PadRight(col1Width) + cells[1].PadLeft(14);
        }
        if (cells.Count == 3)
        {
            int col1Width = totalWidth - 22; // 18 chars
            string c1 = cells[0].Length > col1Width ? cells[0].Substring(0, col1Width) : cells[0];
            return c1.PadRight(col1Width) + cells[1].PadLeft(11) + cells[2].PadLeft(11);
        }
        if (cells.Count == 4)
        {
            int col1Width = totalWidth - 27; // 13 chars
            string c1 = cells[0].Length > col1Width ? cells[0].Substring(0, col1Width) : cells[0];
            return c1.PadRight(col1Width) + cells[1].PadLeft(9) + cells[2].PadLeft(9) + cells[3].PadLeft(9);
        }

        int w = Math.Max(6, totalWidth / cells.Count);
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < cells.Count; i++)
        {
            string c = cells[i].Length > w ? cells[i].Substring(0, w) : cells[i];
            if (i == cells.Count - 1) sb.Append(c.PadLeft(w));
            else sb.Append(c.PadRight(w));
        }
        return sb.ToString();
    }
}

