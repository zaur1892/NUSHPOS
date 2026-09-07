using NUSHPOS.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NUSHPOS.Views.Dialogs;

public partial class CustomerSelectionDialog : Window
{
    public Customer? SelectedCustomer { get; private set; }

    public CustomerSelectionDialog(string phoneNumber, List<Customer> customers)
    {
        InitializeComponent();
        txtNotice.Text = $"{phoneNumber} NÖMRƏSİ BİRDƏN ÇOX ÜNVANDA QEYDİYYATDADIR. ZƏHMƏT OLMASA ÜNVANI SEÇİN.";
        icCustomers.ItemsSource = customers;
    }

    private void BtnCustomerSelect_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Customer customer)
        {
            SelectedCustomer = customer;
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
