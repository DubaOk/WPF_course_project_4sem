using System;
using System.Globalization;
using System.Windows.Data;

namespace Coach_search.MVVM.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isClient && parameter is string options)
            {
                var parts = options.Split('|');
                if (parts.Length == 2)
                {
                    return isClient ? parts[0] : parts[1];
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}