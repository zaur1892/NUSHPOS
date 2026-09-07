using NUSHPOS.Models;
using NUSHPOS.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NUSHPOS.Views.Dialogs;

public partial class CustomerCardDialog : Window
{
    private readonly CustomerService _customerService;
    public Customer CurrentCustomer { get; private set; }
    public string PhoneNumber { get; private set; }
    private TextBox? _lastFocusedTextBox;
    private bool _isCaps = true;

    public CustomerCardDialog(CustomerService customerService, Customer customer, string phoneNumber)
    {
        InitializeComponent();
        _customerService = customerService;
        CurrentCustomer = customer;
        PhoneNumber = phoneNumber;

        if (string.IsNullOrWhiteSpace(CurrentCustomer.DisplayPhoneNumber))
            CurrentCustomer.DisplayPhoneNumber = phoneNumber;

        DataContext = CurrentCustomer;
        _lastFocusedTextBox = txtCustomerName;

        Loaded += (s, e) =>
        {
            txtCustomerName.Focus();
            txtCustomerName.CaretIndex = txtCustomerName.Text.Length;
        };
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            _lastFocusedTextBox = tb;
        }
    }

    private void BtnKey_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content is string keyStr && _lastFocusedTextBox != null)
        {
            string charToAdd = _isCaps ? keyStr.ToUpper() : keyStr.ToLower();
            int caret = _lastFocusedTextBox.CaretIndex;
            _lastFocusedTextBox.Text = _lastFocusedTextBox.Text.Insert(caret, charToAdd);
            _lastFocusedTextBox.CaretIndex = caret + charToAdd.Length;
            _lastFocusedTextBox.Focus();
        }
    }

    private void BtnSpace_Click(object sender, RoutedEventArgs e)
    {
        if (_lastFocusedTextBox != null)
        {
            int caret = _lastFocusedTextBox.CaretIndex;
            _lastFocusedTextBox.Text = _lastFocusedTextBox.Text.Insert(caret, " ");
            _lastFocusedTextBox.CaretIndex = caret + 1;
            _lastFocusedTextBox.Focus();
        }
    }

    private void BtnBackKey_Click(object sender, RoutedEventArgs e)
    {
        if (_lastFocusedTextBox != null && _lastFocusedTextBox.Text.Length > 0 && _lastFocusedTextBox.CaretIndex > 0)
        {
            int caret = _lastFocusedTextBox.CaretIndex;
            _lastFocusedTextBox.Text = _lastFocusedTextBox.Text.Remove(caret - 1, 1);
            _lastFocusedTextBox.CaretIndex = Math.Max(0, caret - 1);
            _lastFocusedTextBox.Focus();
        }
    }

    private void BtnCaps_Click(object sender, RoutedEventArgs e)
    {
        _isCaps = !_isCaps;
    }

    private async void BtnStartOrder_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CurrentCustomer.CustomerName))
        {
            PosMessageDialog.ShowWarning("Zəhmət olmasa müştərinin adını daxil edin!", "XƏBƏRDARLIQ", 5);
            txtCustomerName.Focus();
            return;
        }

        try
        {
            // Save or update customer in database
            CurrentCustomer = await _customerService.SaveOrUpdateCustomerAsync(CurrentCustomer, PhoneNumber);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            PosMessageDialog.ShowError($"Müştəri məlumatı yadda saxlanılarkən xəta baş verdi:\n{ex.Message}", "XƏTA", 10);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
