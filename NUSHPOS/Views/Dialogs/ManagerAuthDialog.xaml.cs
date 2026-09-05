using System.Windows;
using System.Windows.Input;
using NUSHPOS.ViewModels;

namespace NUSHPOS.Views.Dialogs;

public partial class ManagerAuthDialog : Window
{
    public ManagerAuthDialog()
    {
        InitializeComponent();
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
        if (DataContext is not ManagerAuthDialogViewModel vm) return;

        // Number keys (top row: 0..9)
        if (e.Key >= Key.D0 && e.Key <= Key.D9)
        {
            vm.NumpadCommand.Execute((e.Key - Key.D0).ToString());
            e.Handled = true;
        }
        // Numpad keys (keypad: 0..9)
        else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
        {
            vm.NumpadCommand.Execute((e.Key - Key.NumPad0).ToString());
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
        // Enter -> Confirm
        else if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            vm.ConfirmCommand.Execute(null);
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
