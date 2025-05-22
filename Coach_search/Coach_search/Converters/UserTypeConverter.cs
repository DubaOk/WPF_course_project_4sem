using System;
using System.Globalization;
using System.Windows.Data;
using Coach_search.Models;

namespace Coach_search.Converters
{
    public class UserTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var userType = (UserType)value;
            return userType switch
            {
                UserType.Admin => "Администратор",
                UserType.Client => "Клиент",
                UserType.Tutor => "Репетитор",
                _ => string.Empty
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 