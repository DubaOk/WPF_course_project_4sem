using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Coach_search.Converters
{
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status && parameter is string expectedStatus)
            {
                System.Diagnostics.Debug.WriteLine($"StatusToVisibilityConverter: Status={status}, Expected={expectedStatus}, Result={(status == expectedStatus)}");
                return status == expectedStatus ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}