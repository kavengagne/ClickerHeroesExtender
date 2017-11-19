using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Extender.Main.Converters
{
    [ValueConversion(typeof(Visibility), typeof(Visibility))]
    public class ReverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = value as Visibility? ?? Visibility.Hidden;
            if (visiblity == Visibility.Collapsed || visiblity == Visibility.Hidden)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
