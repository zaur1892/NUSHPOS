using NUSHPOS.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NUSHPOS.Views.Dialogs;

public partial class ExpectedPaymentDialog : Window
{
    public PaymentMethod? SelectedPaymentMethod { get; private set; }
    public string SelectedPaymentName => SelectedPaymentMethod?.PaymentName ?? "";

    public ExpectedPaymentDialog(IEnumerable<PaymentMethod> paymentMethods)
    {
        InitializeComponent();
        icPaymentMethods.ItemsSource = paymentMethods;
    }

    private void BtnPaymentSelect_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PaymentMethod pm)
        {
            SelectedPaymentMethod = pm;
            DialogResult = true;
            Close();
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
