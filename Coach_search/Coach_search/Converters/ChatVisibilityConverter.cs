using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Coach_search.Converters
{
    public class ChatVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                bool isVisible = status != "Отклонено";
                System.Diagnostics.Debug.WriteLine($"ChatVisibilityConverter: Status={status}, IsVisible={isVisible}");
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            System.Diagnostics.Debug.WriteLine($"ChatVisibilityConverter: Invalid status value={value}");
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}