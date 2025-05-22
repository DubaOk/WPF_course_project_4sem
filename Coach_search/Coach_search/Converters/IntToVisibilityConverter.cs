using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Coach_search.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            int count = (int)value;
            int compareValue = int.Parse(parameter.ToString());

            // Если compareValue = 0, показываем элемент когда count = 0
            // Если compareValue = 1, показываем элемент когда count > 0
            if (compareValue == 0)
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            else
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 