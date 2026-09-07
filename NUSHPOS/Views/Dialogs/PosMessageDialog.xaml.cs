using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NUSHPOS.Views.Dialogs;

public enum PosMessageType
{
    Warning,
    Information,
    Confirmation,
    Error,
    Success
}

public partial class PosMessageDialog : Window
{
    private readonly DispatcherTimer _timer;
    private int _countdown;
    private readonly PosMessageType _type;
    private readonly bool _isConfirmation;

    public PosMessageDialog(
        string message, 
        string title = "", 
        PosMessageType type = PosMessageType.Warning, 
        int countdownSeconds = 20, 
        string details = "", 
        bool isConfirmation = false)
    {
        InitializeComponent();

        _type = type;
        _isConfirmation = isConfirmation;
        _countdown = countdownSeconds;

        SetupTypeStyling(type, title);

        TxtMessage.Text = message;

        if (!string.IsNullOrWhiteSpace(details))
        {
            TxtDetails.Text = details;
            TxtDetails.Visibility = Visibility.Visible;
        }
        else
        {
            TxtDetails.Visibility = Visibility.Collapsed;
        }

        if (_isConfirmation)
        {
            PanelOk.Visibility = Visibility.Collapsed;
            PanelYesNo.Visibility = Visibility.Visible;
        }
        else
        {
            PanelOk.Visibility = Visibility.Visible;
            PanelYesNo.Visibility = Visibility.Collapsed;
        }

        // Setup Countdown Timer
        if (_countdown > 0)
        {
            TxtCountdown.Text = _countdown.ToString();
            CountdownBadge.Visibility = Visibility.Visible;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }
        else
        {
            CountdownBadge.Visibility = Visibility.Collapsed;
            _timer = new DispatcherTimer(); // empty non-started timer
        }
    }

    private void SetupTypeStyling(PosMessageType type, string customTitle)
    {
        switch (type)
        {
            case PosMessageType.Warning:
                TxtTitle.Text = string.IsNullOrWhiteSpace(customTitle) ? "XƏBƏRDARLIQ BİLDİRİŞİ" : customTitle;
                TxtTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"));
                IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
                TxtIconSymbol.Text = "!";
                break;

            case PosMessageType.Information:
                TxtTitle.Text = string.IsNullOrWhiteSpace(customTitle) ? "MƏLUMAT BİLDİRİŞİ" : customTitle;
                TxtTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"));
                IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0284C7"));
                TxtIconSymbol.Text = "ℹ";
                break;

            case PosMessageType.Confirmation:
                TxtTitle.Text = string.IsNullOrWhiteSpace(customTitle) ? "TƏSDİQ BİLDİRİŞİ" : customTitle;
                TxtTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"));
                IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                TxtIconSymbol.Text = "?";
                break;

            case PosMessageType.Error:
                TxtTitle.Text = string.IsNullOrWhiteSpace(customTitle) ? "XƏTA BİLDİRİŞİ" : customTitle;
                TxtTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#991B1B"));
                IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
                TxtIconSymbol.Text = "✕";
                break;

            case PosMessageType.Success:
                TxtTitle.Text = string.IsNullOrWhiteSpace(customTitle) ? "UĞURLU ƏMƏLİYYAT" : customTitle;
                TxtTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#065F46"));
                IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                TxtIconSymbol.Text = "✓";
                break;
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (_countdown > 0)
        {
            _timer.Start();
        }

        if (_isConfirmation)
        {
            BtnYes.Focus();
        }
        else
        {
            BtnOk.Focus();
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_countdown > 1)
        {
            _countdown--;
            TxtCountdown.Text = _countdown.ToString();
        }
        else
        {
            _timer.Stop();
            // Default close on timeout
            if (_isConfirmation)
            {
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
            }
            Close();
        }
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            try { DragMove(); } catch { }
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        _timer?.Stop();
        DialogResult = true;
        Close();
    }

    private void BtnYes_Click(object sender, RoutedEventArgs e)
    {
        _timer?.Stop();
        DialogResult = true;
        Close();
    }

    private void BtnNo_Click(object sender, RoutedEventArgs e)
    {
        _timer?.Stop();
        DialogResult = false;
        Close();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _timer?.Stop();
            DialogResult = true;
            Close();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            _timer?.Stop();
            DialogResult = false;
            Close();
            e.Handled = true;
        }
        else if (_isConfirmation)
        {
            if (e.Key == Key.Y || e.Key == Key.B)
            {
                _timer?.Stop();
                DialogResult = true;
                Close();
                e.Handled = true;
            }
            else if (e.Key == Key.N || e.Key == Key.X)
            {
                _timer?.Stop();
                DialogResult = false;
                Close();
                e.Handled = true;
            }
        }
    }

    #region Static Helper Methods

    /// <summary>
    /// Shows a POS Warning dialog with countdown (default 20 seconds).
    /// </summary>
    public static bool? ShowWarning(string message, string title = "XƏBƏRDARLIQ BİLDİRİŞİ", int countdownSeconds = 20, string details = "", Window? owner = null)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var dlg = new PosMessageDialog(message, title, PosMessageType.Warning, countdownSeconds, details, isConfirmation: false);
            if (owner != null) dlg.Owner = owner;
            else if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsVisible) dlg.Owner = Application.Current.MainWindow;
            return dlg.ShowDialog();
        });
    }

    /// <summary>
    /// Shows a POS Information dialog with countdown (default 15 seconds).
    /// </summary>
    public static bool? ShowInfo(string message, string title = "MƏLUMAT BİLDİRİŞİ", int countdownSeconds = 15, string details = "", Window? owner = null)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var dlg = new PosMessageDialog(message, title, PosMessageType.Information, countdownSeconds, details, isConfirmation: false);
            if (owner != null) dlg.Owner = owner;
            else if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsVisible) dlg.Owner = Application.Current.MainWindow;
            return dlg.ShowDialog();
        });
    }

    /// <summary>
    /// Shows a POS Success dialog with countdown (default 10 seconds).
    /// </summary>
    public static bool? ShowSuccess(string message, string title = "UĞURLU ƏMƏLİYYAT", int countdownSeconds = 10, string details = "", Window? owner = null)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var dlg = new PosMessageDialog(message, title, PosMessageType.Success, countdownSeconds, details, isConfirmation: false);
            if (owner != null) dlg.Owner = owner;
            else if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsVisible) dlg.Owner = Application.Current.MainWindow;
            return dlg.ShowDialog();
        });
    }

    /// <summary>
    /// Shows a POS Error dialog with countdown (default 20 seconds).
    /// </summary>
    public static bool? ShowError(string message, string title = "XƏTA BİLDİRİŞİ", int countdownSeconds = 20, string details = "", Window? owner = null)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var dlg = new PosMessageDialog(message, title, PosMessageType.Error, countdownSeconds, details, isConfirmation: false);
            if (owner != null) dlg.Owner = owner;
            else if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsVisible) dlg.Owner = Application.Current.MainWindow;
            return dlg.ShowDialog();
        });
    }

    /// <summary>
    /// Shows a POS Confirmation dialog with Yes (Bəli) / No (Xeyr) buttons and countdown (default 20 seconds).
    /// </summary>
    public static bool ShowConfirm(string message, string title = "TƏSDİQ BİLDİRİŞİ", int countdownSeconds = 20, string details = "", Window? owner = null)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var dlg = new PosMessageDialog(message, title, PosMessageType.Confirmation, countdownSeconds, details, isConfirmation: true);
            if (owner != null) dlg.Owner = owner;
            else if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsVisible) dlg.Owner = Application.Current.MainWindow;
            return dlg.ShowDialog() == true;
        });
    }

    #endregion
}
