using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NUSHPOS.Converters;

public class StringToColorBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var defaultBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71")); // Default Green
        if (value == null) return defaultBrush;

        string colorString = value.ToString() ?? "";
        if (string.IsNullOrWhiteSpace(colorString)) return defaultBrush;

            // Handle specific "iButton..." string formats from the database
            if (colorString.StartsWith("iButton", StringComparison.OrdinalIgnoreCase))
            {
                string buttonColor = colorString.Substring(7).ToLower();
                return buttonColor switch
                {
                    "blue" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                    "red" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")),
                    "green" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71")),
                    "yellow" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F")),
                    "orange" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E67E22")),
                    "purple" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B59B6")),
                    "default" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6")),
                    _ => defaultBrush
                };
            }

            // If it's a hex color or named color (that WPF recognizes)
            if (colorString.StartsWith("#") || (char.IsLetter(colorString[0]) && !colorString.Contains("\\")))
            {
                try 
                {
                    var converted = ColorConverter.ConvertFromString(colorString);
                    if (converted != null)
                    {
                        return new SolidColorBrush((Color)converted);
                    }
                }
                catch { }
            }
            
            // If it's an integer (like ARGB or Win32 COLORREF)
            if (int.TryParse(colorString, out int colorInt))
            {
                // Typically from VB/Delphi, colors are BGR or integer ARGB
                byte[] bytes = BitConverter.GetBytes(colorInt);
                // Windows COLORREF is 0x00bbggrr
                // Let's assume standard ARGB first, or BGR if A is 0
                byte a = bytes[3] == 0 ? (byte)255 : bytes[3];
                byte r = bytes[0];
                byte g = bytes[1];
                byte b = bytes[2];
                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }


        return defaultBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
