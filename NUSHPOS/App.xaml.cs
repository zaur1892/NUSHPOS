using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NUSHPOS.Helpers;
using NUSHPOS.Services;
using NUSHPOS.ViewModels;
using NUSHPOS.Views;

namespace NUSHPOS;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public static ServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Dapper.SqlMapper.AddTypeHandler(new DapperGuidHandler());

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
        Services = _serviceProvider;

        var mainWindow = new MainWindow();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<DatabaseService>();
        services.AddSingleton<NavigationService>();
        services.AddTransient<AccessLogService>();
        services.AddTransient<EmployeeService>();
        services.AddTransient<TableService>();
        services.AddTransient<MenuService>();
        services.AddTransient<OrderService>();
        services.AddTransient<PaymentService>();
        services.AddTransient<CustomerService>();
        services.AddTransient<DiscountService>();
        services.AddTransient<RegisterSessionService>();
        services.AddTransient<SettingsService>();
        services.AddTransient<StationSettingsService>();
        services.AddTransient<FastReportService>();
        services.AddTransient<AuthorityService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainScreenViewModel>();
        services.AddTransient<TablePlanViewModel>();
        services.AddTransient<SaleScreenViewModel>();
        services.AddTransient<OperationsViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<ReportEditorViewModel>();
        services.AddTransient<OrderRecallViewModel>();
        services.AddTransient<BackOfficeViewModel>();
        services.AddTransient<CompanySettingsViewModel>();
        services.AddTransient<EmployeeSettingsViewModel>();
        services.AddTransient<MenuDesignerViewModel>();
        services.AddTransient<TerminalSettingsViewModel>();
        services.AddTransient<SecuritySettingsViewModel>();
        services.AddSingleton<MainWindowViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

public class DapperGuidHandler : Dapper.SqlMapper.TypeHandler<string>
{
    public override void SetValue(System.Data.IDbDataParameter parameter, string? value)
    {
        parameter.Value = value ?? (object)System.DBNull.Value;
    }

    public override string? Parse(object value)
    {
        return value?.ToString();
    }
}

