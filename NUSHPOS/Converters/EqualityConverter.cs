using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NUSHPOS.Converters;

public class EqualityConverter : IValueConverter, IMultiValueConverter
{
    // IValueConverter (Single binding with ConverterParameter)
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool areEqual = false;
        if (value == null && parameter == null)
        {
            areEqual = true;
        }
        else if (value != null && parameter != null)
        {
            if (value.Equals(parameter) || string.Equals(value.ToString(), parameter.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                areEqual = true;
            }
        }

        if (targetType == typeof(Visibility))
        {
            return areEqual ? Visibility.Visible : Visibility.Collapsed;
        }

        return areEqual;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    // IMultiValueConverter (MultiBinding comparing multiple values)
    public object Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2)
            return targetType == typeof(Visibility) ? Visibility.Collapsed : false;

        bool areEqual = Equals(values[0], values[1]) || (values[0]?.ToString() == values[1]?.ToString());

        if (targetType == typeof(Visibility))
        {
            return areEqual ? Visibility.Visible : Visibility.Collapsed;
        }

        return areEqual;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
