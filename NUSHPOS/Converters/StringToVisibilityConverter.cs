using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NUSHPOS.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        // If string is null or empty, return Visible (so placeholder shows), else Collapsed
        // Or vice versa depending on how it's used. Let's make it configurable with parameter "Inverted".
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            bool isEmpty = string.IsNullOrEmpty(str);
            
            bool inverted = parameter != null && parameter.ToString().Equals("Inverted", StringComparison.OrdinalIgnoreCase);

            if (inverted)
            {
                return isEmpty ? Visibility.Collapsed : Visibility.Visible;
            }
            
            return isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
