using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using NUSHPOS.Helpers;
using NUSHPOS.ViewModels.Base;
using NUSHPOS.Views;
using System;

namespace NUSHPOS.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private object? _currentView;

    public MainWindowViewModel(IServiceProvider serviceProvider, NavigationService navigationService)
    {
        _serviceProvider = serviceProvider;
        _navigationService = navigationService;
        
        _navigationService.NavigationRequested += OnNavigationRequested;
        _navigationService.NavigationWithParamRequested += OnNavigationWithParamRequested;
        
        // Start with Login
        NavigateTo(typeof(LoginViewModel), null);
    }

    private void OnNavigationRequested(Type viewModelType)
    {
        NavigateTo(viewModelType, null);
    }

    private void OnNavigationWithParamRequested(Type viewModelType, object? parameter)
    {
        NavigateTo(viewModelType, parameter);
    }

    private void NavigateTo(Type viewModelType, object? parameter)
    {
        if (viewModelType == typeof(LoginViewModel))
        {
            var vm = _serviceProvider.GetRequiredService<LoginViewModel>();
            CurrentView = new LoginView { DataContext = vm };
        }
        else if (viewModelType == typeof(MainScreenViewModel))
        {
            var vm = _serviceProvider.GetRequiredService<MainScreenViewModel>();
            CurrentView = new MainScreenView { DataContext = vm };
        }
        else if (viewModelType == typeof(TablePlanViewModel))
        {
            var vm = _serviceProvider.GetRequiredService<TablePlanViewModel>();
            CurrentView = new TablePlanView { DataContext = vm };
        }
        else if (viewModelType == typeof(SaleScreenViewModel))
        {
            var vm = _serviceProvider.GetRequiredService<SaleScreenViewModel>();
            if (parameter != null)
            {
                vm.Initialize(parameter);
            }
            CurrentView = new SaleScreenView { DataContext = vm };
        }
        else if (viewModelType == typeof(OperationsViewModel))
        {
            var vm = _serviceProvider.GetRequiredService<OperationsViewModel>();
            CurrentView = new OperationsView { DataContext = vm };
        }
        else if (viewModelType == typeof(ReportsViewModel))
        {
            var vm = _serviceProvider.GetRequiredService<ReportsViewModel>();
            CurrentView = new ReportsView { DataContext = vm };
        }
        else if (viewModelType == typeof(OrderRecallViewModel))
        {
            var vm = _serviceProvider.GetRequiredService<OrderRecallViewModel>();
            CurrentView = new OrderRecallView { DataContext = vm };
        }
        else if (viewModelType == typeof(BackOfficeViewModel))
        {
            var vm = _serviceProvider.GetRequiredService<BackOfficeViewModel>();
            CurrentView = new BackOfficeView { DataContext = vm };
        }
    }
}
