using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NUSHPOS.Converters;

public class ZeroToVisibilityConverter : IValueConverter
{
    public Visibility ZeroVisibility { get; set; } = Visibility.Collapsed;
    public Visibility NonZeroVisibility { get; set; } = Visibility.Visible;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal d && d == 0)
            return ZeroVisibility;
        if (value is double db && db == 0)
            return ZeroVisibility;
        if (value is int i && i == 0)
            return ZeroVisibility;

        return NonZeroVisibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
