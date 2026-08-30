using System.Windows.Controls;
using System.Windows.Input;
using NUSHPOS.ViewModels;

namespace NUSHPOS.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        this.Loaded += (s, e) => this.Focus();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
        if (this.DataContext is LoginViewModel vm)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                vm.NumpadCommand.Execute((e.Key - Key.D0).ToString());
                e.Handled = true;
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                vm.NumpadCommand.Execute((e.Key - Key.NumPad0).ToString());
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                vm.BackspaceCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                vm.LoginCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                vm.ClearCommand.Execute(null);
                e.Handled = true;
            }
        }


    }
}
