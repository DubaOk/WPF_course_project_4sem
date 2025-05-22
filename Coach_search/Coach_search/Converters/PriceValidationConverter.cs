using System;
using System.Globalization;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace Coach_search.MVVM.View
{
    public class PriceValidationConverter : IValueConverter
    {
        private static readonly Regex DecimalRegex = new Regex(@"^\d+(\.\d{0,2})?$");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string priceStr = value.ToString();
            
            // Проверяем, что строка соответствует формату decimal с максимум 2 знаками после точки
            if (!DecimalRegex.IsMatch(priceStr))
                return false;

            // Пробуем преобразовать в decimal
            if (decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                return price > 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 