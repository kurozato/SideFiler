using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BlackSugar.Views
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource? source = value as BitmapSource;

            if (source == null) return DependencyProperty.UnsetValue;

            return new System.Windows.Controls.Image() { 
                    Source = source,
                    Height = 18,
                    Width = 18,
                };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
