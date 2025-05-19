using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Coach_search.Converters
{
    public class BooleanToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSentByCurrentUser && parameter is string colors)
            {
                var colorArray = colors.Split(';');
                if (colorArray.Length == 2)
                {
                    var color = isSentByCurrentUser ? colorArray[0] : colorArray[1];
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                }
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}