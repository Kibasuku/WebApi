using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ClientApp.Converter
{
    public class Base64ToImgConverter : IValueConverter
    {
        public int Id { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] binaryData = System.Convert.FromBase64String(value.ToString());
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            return bi;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}