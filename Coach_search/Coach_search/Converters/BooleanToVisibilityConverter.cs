using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Coach_search.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                System.Diagnostics.Debug.WriteLine($"BooleanToVisibilityConverter: value={boolValue}");
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            System.Diagnostics.Debug.WriteLine($"BooleanToVisibilityConverter: value is not bool, type={value?.GetType()}, returning Collapsed");
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}