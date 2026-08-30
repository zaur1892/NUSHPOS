using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace NUSHPOS.Converters;

public class ImageFileToSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string fileName)
        {
            try
            {
                // Check bin/Debug/Images or bin/Release/Images
                string path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                // Check project root Images folder (useful during development)
                string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Images", fileName);
                // For modern .NET where it might be bin/Debug/net6.0-windows etc.
                string path3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Images", fileName);

                string foundPath = null;
                if (File.Exists(path1)) foundPath = path1;
                else if (File.Exists(path2)) foundPath = path2;
                else if (File.Exists(path3)) foundPath = path3;

                if (foundPath != null)
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = new Uri(foundPath, UriKind.Absolute);
                    bmp.EndInit();
                    return bmp;
                }
            }
            catch
            {
                // If any error occurs, just return null so it falls back gracefully
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
