using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using NUSHPOS.Services;
using System.Data;

using NUSHPOS.ViewModels.Base;

namespace NUSHPOS.ViewModels
{
    public partial class ReportEditorViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;

        public event EventHandler? RequestClose;

        [ObservableProperty]
        private bool _isNewReport;

        [ObservableProperty]
        private int _reportID;

        [ObservableProperty]
        private string _reportKey = Guid.NewGuid().ToString().ToUpper();

        [ObservableProperty]
        private string _reportName = string.Empty;

        [ObservableProperty]
        private int _reportCategoryID;

        [ObservableProperty]
        private int _reportTypeID = 2; // Default to Liste (2)

        [ObservableProperty]
        private int _securityLevel = 1;

        [ObservableProperty]
        private bool _isPosReport;

        [ObservableProperty]
        private ObservableCollection<ReportCategoryModel> _categories = new();

        [ObservableProperty]
        private ObservableCollection<ReportQueryEditorModel> _queries = new();

        [ObservableProperty]
        private ReportQueryEditorModel? _selectedQuery;

        [ObservableProperty]
        private string _designTemplate = string.Empty;

        [ObservableProperty]
        private DataTable? _previewData;

        [ObservableProperty]
        private string _testErrorMessage = string.Empty;

        public ReportEditorViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task LoadCategoriesAsync()
        {
            using var conn = _databaseService.CreateConnection();
            string sql = "SELECT [ReportCategoryID], [CategoryName] FROM [ReportCategories] WHERE [MainCategoryID] IS NULL OR [MainCategoryID] = 0";
            var cats = await conn.QueryAsync<ReportCategoryModel>(sql);
            Categories = new ObservableCollection<ReportCategoryModel>(cats);

            if (IsNewReport && Categories.Any())
            {
                ReportCategoryID = Categories.First().ReportCategoryID;
            }
        }

        public async Task LoadReportAsync(int reportId)
        {
            using var conn = _databaseService.CreateConnection();
            
            string repSql = "SELECT ReportID, ReportKey, ReportName, ReportCategoryID, ReportTypeID, SecurityLevel, IsPosReport FROM Reports WHERE ReportID = @ReportID";
            var rep = await conn.QueryFirstOrDefaultAsync<dynamic>(repSql, new { ReportID = reportId });
            
            if (rep != null)
            {
                IsNewReport = false;
                ReportID = rep.ReportID;
                ReportKey = rep.ReportKey?.ToString() ?? "";
                ReportName = rep.ReportName;
                ReportCategoryID = rep.ReportCategoryID;
                ReportTypeID = rep.ReportTypeID;
                SecurityLevel = rep.SecurityLevel;
                IsPosReport = rep.IsPosReport;
                
                string querySql = "SELECT ReportQueryID, CAST(ReportQueryKey AS NVARCHAR(50)) AS ReportQueryKey, QueryName, QueryData, IsDefault FROM ReportQueries WHERE ReportID = @ReportID";
                var allQueries = await conn.QueryAsync<ReportQueryEditorModel>(querySql, new { ReportID = reportId });
                
                var designSql = "SELECT DesignData FROM ReportDesigns WHERE ReportID = @ReportID";
                var designBytes = await conn.QueryFirstOrDefaultAsync<byte[]>(designSql, new { ReportID = reportId });
                if (designBytes != null && designBytes.Length > 0)
                {
                    try {
                        string decoded = System.Text.Encoding.UTF8.GetString(designBytes);
                        if (!decoded.StartsWith("/// <XRTypeInfo>"))
                            DesignTemplate = decoded;
                        else
                            DesignTemplate = string.Empty;
                    } catch {
                        DesignTemplate = string.Empty;
                    }
                }
                else
                {
                    DesignTemplate = string.Empty;
                }
                
                Queries = new System.Collections.ObjectModel.ObservableCollection<ReportQueryEditorModel>(allQueries);
                
                if (Queries.Any()) SelectedQuery = Queries.First();
            }
        }

        [RelayCommand]
        private void AddQuery()
        {
            var newQuery = new ReportQueryEditorModel
            {
                ReportQueryKey = Guid.NewGuid().ToString().ToUpper(),
                QueryName = "New Query",
                QueryData = "SELECT GETDATE()",
                IsDefault = 0
            };
            Queries.Add(newQuery);
            SelectedQuery = newQuery;
        }

        [RelayCommand]
        private void DeleteQuery()
        {
            if (SelectedQuery != null)
            {
                Queries.Remove(SelectedQuery);
                SelectedQuery = Queries.FirstOrDefault();
            }
        }

        [RelayCommand]
        private async Task TestQueryAsync()
        {
            if (SelectedQuery == null || string.IsNullOrWhiteSpace(SelectedQuery.QueryData)) return;

            TestErrorMessage = string.Empty;
            PreviewData = null;

            try
            {
                using var conn = _databaseService.CreateConnection();
                // We do not replace Global... here because we want them to write normal SQL, 
                // but if they write Global..., they might get an error testing unless we replace it here too.
                string queryToTest = SelectedQuery.QueryData
                    .Replace("GlobalPaymentMethods", "PaymentMethods")
                    .Replace("GlobalEmployeeFiles", "EmployeeFiles")
                    .Replace("GlobalMenuItems", "MenuItems")
                    .Replace("GlobalPromotions", "Promotions")
                    .Replace("GlobalDiscounts", "Discounts");

                var dt = new DataTable();
                using (var reader = await conn.ExecuteReaderAsync(queryToTest, new { date1 = DateTime.Today, date2 = DateTime.Now }))
                {
                    dt.Load(reader);
                }
                PreviewData = dt;
            }
            catch (Exception ex)
            {
                TestErrorMessage = ex.Message;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            await SaveToDatabaseAsync();
        }

        [RelayCommand]
        private async Task SaveAndCloseAsync()
        {
            await SaveToDatabaseAsync();
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private async Task SaveToDatabaseAsync()
        {
            using var conn = _databaseService.CreateConnection();

            if (IsNewReport)
            {
                // Generate a new ReportID
                ReportID = await conn.QuerySingleAsync<int>("SELECT ISNULL(MAX(ReportID), 0) + 1 FROM Reports");
                
                string insertSql = @"
                    INSERT INTO Reports (ReportID, ReportKey, ReportName, ReportCategoryID, ReportActive, SecurityLevel, ReportTypeID, IsPosReport)
                    VALUES (@ReportID, @ReportKey, @ReportName, @ReportCategoryID, 1, @SecurityLevel, @ReportTypeID, @IsPosReport)";
                await conn.ExecuteAsync(insertSql, new { ReportID, ReportKey, ReportName, ReportCategoryID, SecurityLevel, ReportTypeID, IsPosReport });
                IsNewReport = false;
            }
            else
            {
                string updateSql = @"
                    UPDATE Reports 
                    SET ReportName = @ReportName, ReportCategoryID = @ReportCategoryID, 
                        SecurityLevel = @SecurityLevel, ReportTypeID = @ReportTypeID, IsPosReport = @IsPosReport
                    WHERE ReportID = @ReportID";
                await conn.ExecuteAsync(updateSql, new { ReportName, ReportCategoryID, SecurityLevel, ReportTypeID, IsPosReport, ReportID });
            }

            // Save Queries
            // Delete existing queries for this report first to handle deletions
            await conn.ExecuteAsync("DELETE FROM ReportQueries WHERE ReportID = @ReportID", new { ReportID });

            string insertQuerySql = @"
                INSERT INTO ReportQueries (ReportQueryID, ReportQueryKey, ReportID, ReportKey, QueryName, QueryData, IsDefault)
                VALUES (@ReportQueryID, @ReportQueryKey, @ReportID, @ReportKey, @QueryName, @QueryData, @IsDefault)";
            
            foreach (var q in Queries)
            {
                await conn.ExecuteAsync(insertQuerySql, new { 
                    ReportQueryID = 0, // In user's DB dump, this is always 0
                    ReportQueryKey = q.ReportQueryKey,
                    ReportID = this.ReportID,
                    ReportKey = this.ReportKey,
                    QueryName = q.QueryName,
                    QueryData = q.QueryData,
                    IsDefault = q.IsDefault
                });
            }

            // Save Design to ReportDesigns
            await conn.ExecuteAsync("DELETE FROM ReportDesigns WHERE ReportID = @ReportID", new { ReportID });

            if (!string.IsNullOrWhiteSpace(DesignTemplate))
            {
                string insertDesignSql = @"
                    INSERT INTO ReportDesigns (ReportDesignKey, ReportID, ReportKey, DocumentTypeID, DesignName, DesignData, IsDefault)
                    VALUES (NEWID(), @ReportID, @ReportKey, 0, @DesignName, @DesignData, 0)";
                
                byte[] designBytes = System.Text.Encoding.UTF8.GetBytes(DesignTemplate);
                await conn.ExecuteAsync(insertDesignSql, new { ReportID, ReportKey, DesignName = ReportName, DesignData = designBytes });
            }
        }
    }

    public partial class ReportQueryEditorModel : ObservableObject
    {
        [ObservableProperty]
        private string _reportQueryKey = string.Empty;

        [ObservableProperty]
        private string _queryName = string.Empty;

        [ObservableProperty]
        private string _queryData = string.Empty;

        [ObservableProperty]
        private int _isDefault;
    }
}
