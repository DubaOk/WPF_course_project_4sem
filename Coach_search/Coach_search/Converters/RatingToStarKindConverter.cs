using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace Coach_search.MVVM.View
{
    public class RatingToStarKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double rating && parameter is string position)
            {
                int starPosition = int.Parse(position);
                return rating >= starPosition ? PackIconKind.Star : PackIconKind.StarOutline;
            }
            return PackIconKind.StarOutline;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 