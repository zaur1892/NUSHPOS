using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NUSHPOS.Views.Dialogs;

public partial class PhoneEntryDialog : Window
{
    public string PhoneNumber { get; set; } = "";

    public PhoneEntryDialog(string initialPhone = "")
    {
        InitializeComponent();
        PhoneNumber = initialPhone;
        txtPhone.Text = PhoneNumber;
        Loaded += (s, e) =>
        {
            txtPhone.Focus();
            txtPhone.CaretIndex = txtPhone.Text.Length;
        };
    }

    private void BtnNum_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content is string num)
        {
            txtPhone.Text += num;
            PhoneNumber = txtPhone.Text;
            txtPhone.CaretIndex = txtPhone.Text.Length;
        }
    }

    private void BtnBackspace_Click(object sender, RoutedEventArgs e)
    {
        if (txtPhone.Text.Length > 0)
        {
            txtPhone.Text = txtPhone.Text.Substring(0, txtPhone.Text.Length - 1);
            PhoneNumber = txtPhone.Text;
            txtPhone.CaretIndex = txtPhone.Text.Length;
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        PhoneNumber = txtPhone.Text.Trim();
        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            PosMessageDialog.ShowWarning("Zəhmət olmasa telefon nömrəsini daxil edin!", "XƏBƏRDARLIQ", 5);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            BtnConfirm_Click(this, new RoutedEventArgs());
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            BtnCancel_Click(this, new RoutedEventArgs());
            e.Handled = true;
        }
    }
}
