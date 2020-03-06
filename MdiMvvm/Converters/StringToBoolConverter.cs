using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MdiMvvm.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string str)) return Visibility.Collapsed;
            if (string.IsNullOrEmpty(str)) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
