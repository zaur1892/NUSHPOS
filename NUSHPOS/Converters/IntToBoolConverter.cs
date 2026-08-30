using System;
using System.Globalization;
using System.Windows.Data;

namespace NUSHPOS.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue && parameter != null)
            {
                if (int.TryParse(parameter.ToString(), out int paramValue))
                {
                    return intValue == paramValue;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter != null)
            {
                if (int.TryParse(parameter.ToString(), out int paramValue))
                {
                    return paramValue;
                }
            }
            return Binding.DoNothing;
        }
    }
}
