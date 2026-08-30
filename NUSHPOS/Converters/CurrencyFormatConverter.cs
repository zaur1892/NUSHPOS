using System;
using System.Globalization;
using System.Windows.Data;

namespace NUSHPOS.Converters;

public class CurrencyFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal amount)
        {
            return $"{amount:N2} ₼";
        }
        if (value is double d)
        {
            return $"{d:N2} ₼";
        }
        return "0.00 ₼";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
