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
                bool isVisible = status == expectedStatus;
                System.Diagnostics.Debug.WriteLine($"StatusToVisibilityConverter: Status={status}, Expected={expectedStatus}, IsVisible={isVisible}");
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            System.Diagnostics.Debug.WriteLine($"StatusToVisibilityConverter: Invalid input - Status={value}, Parameter={parameter}");
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}