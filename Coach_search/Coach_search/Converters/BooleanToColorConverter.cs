using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Coach_search.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string colors)
            {
                var colorParts = colors.Split(',');
                if (colorParts.Length == 2)
                {
                    var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(boolValue ? colorParts[0] : colorParts[1]));
                    brush.Freeze();
                    return brush;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 