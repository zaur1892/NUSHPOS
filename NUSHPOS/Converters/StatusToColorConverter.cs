using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace NUSHPOS.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int status)
        {
            var successColor = (Color)ColorConverter.ConvertFromString("#2ECC71");
            var dangerColor = (Color)ColorConverter.ConvertFromString("#E74C3C");
            
            switch (status)
            {
                case 0:
                    return new SolidColorBrush(Colors.Transparent);
                case 1:
                    return new SolidColorBrush(successColor);
                case 2:
                    return new SolidColorBrush(dangerColor);
                case 3:
                    return new SolidColorBrush(Colors.Transparent);
                default:
                    return new SolidColorBrush(Colors.Gray);
            }
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
