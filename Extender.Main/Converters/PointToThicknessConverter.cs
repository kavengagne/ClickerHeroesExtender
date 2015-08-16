using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Point = System.Drawing.Point;


namespace Extender.Main.Converters
{
    [ValueConversion(typeof(Point), typeof(Thickness))]
    public class PointToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var point = value as Point? ?? new Point();

            int width;
            int.TryParse(parameter as string, out width);
            int offset = width / 2;

            return new Thickness(point.X - offset, point.Y - offset, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var thickness = value as Thickness? ?? new Thickness();

            int width;
            int.TryParse(parameter as string, out width);
            int offset = width / 2;

            return new Point((int)(thickness.Left + offset), (int)(thickness.Top + offset));
        }
    }
}
