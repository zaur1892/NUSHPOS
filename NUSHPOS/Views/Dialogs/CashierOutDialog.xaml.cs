using System.Windows;
using System.Windows.Input;
using NUSHPOS.ViewModels;

namespace NUSHPOS.Views.Dialogs;

public partial class CashierOutDialog : Window
{
    public CashierOutDialog(CashierOutDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.RequestClose = (success) =>
        {
            viewModel.Cleanup();
            DialogResult = success;
            Close();
        };
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Focus();
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
