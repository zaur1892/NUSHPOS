using System.Windows;
using System.Windows.Input;
using NUSHPOS.ViewModels;

namespace NUSHPOS.Views.Dialogs;

public partial class CashierInDialog : Window
{
    public decimal ResultAmount { get; private set; }

    public CashierInDialog()
    {
        InitializeComponent();
        var vm = new CashierInDialogViewModel();
        vm.RequestClose += OnRequestClose;
        DataContext = vm;
    }

    private void OnRequestClose(bool success)
    {
        if (DataContext is CashierInDialogViewModel vm)
        {
            ResultAmount = vm.Amount;
            vm.Cleanup();
        }
        DialogResult = success;
        Close();
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

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not CashierInDialogViewModel vm) return;

        if (vm.IsConfirming)
        {
            if (e.Key == Key.Enter || e.Key == Key.Y || e.Key == Key.B)
            {
                vm.ConfirmYesCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape || e.Key == Key.N || e.Key == Key.X)
            {
                vm.ConfirmNoCommand.Execute(null);
                e.Handled = true;
            }
            return;
        }

        // Top row numbers (0..9)
        if (e.Key >= Key.D0 && e.Key <= Key.D9)
        {
            vm.NumpadCommand.Execute((e.Key - Key.D0).ToString());
            e.Handled = true;
        }
        // Numpad numbers (0..9)
        else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
        {
            vm.NumpadCommand.Execute((e.Key - Key.NumPad0).ToString());
            e.Handled = true;
        }
        // Decimal point / comma
        else if (e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.Decimal)
        {
            vm.NumpadCommand.Execute(".");
            e.Handled = true;
        }
        // Backspace
        else if (e.Key == Key.Back)
        {
            vm.BackspaceCommand.Execute(null);
            e.Handled = true;
        }
        // Escape -> Cancel
        else if (e.Key == Key.Escape)
        {
            vm.CancelCommand.Execute(null);
            e.Handled = true;
        }
        // Enter -> Submit
        else if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            vm.SubmitAmountCommand.Execute(null);
            e.Handled = true;
        }
        // Delete or C -> Clear
        else if (e.Key == Key.Delete || e.Key == Key.C)
        {
            vm.ClearCommand.Execute(null);
            e.Handled = true;
        }
    }
}
