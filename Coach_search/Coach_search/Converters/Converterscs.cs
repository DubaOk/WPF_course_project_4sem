using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Coach_search.Converters
{
    // Конвертер для видимости кнопок на основе статуса
    //public class StatusToVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is string status && parameter is string expectedStatus)
    //        {
    //            System.Diagnostics.Debug.WriteLine($"StatusToVisibilityConverter: Status={status}, Expected={expectedStatus}, Result={(status == expectedStatus)}");
    //            return status == expectedStatus ? Visibility.Visible : Visibility.Collapsed;
    //        }
    //        return Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    // Конвертер для видимости чата на основе статуса
    //public class ChatVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is string status)
    //        {
    //            bool isVisible = status != "Отклонено";
    //            System.Diagnostics.Debug.WriteLine($"ChatVisibilityConverter: Status={status}, IsVisible={isVisible}");
    //            return isVisible ? Visibility.Visible : Visibility.Collapsed;
    //        }
    //        System.Diagnostics.Debug.WriteLine($"ChatVisibilityConverter: Invalid status value={value}");
    //        return Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //// Конвертер для выравнивания текста на основе булева значения
    //public class BooleanToAlignmentConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is bool isSentByCurrentUser)
    //        {
    //            return isSentByCurrentUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    //        }
    //        return HorizontalAlignment.Left;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //// Конвертер для фона на основе булева значения
    //public class BooleanToBackgroundConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is bool isSentByCurrentUser && parameter is string colors)
    //        {
    //            var colorArray = colors.Split(';');
    //            if (colorArray.Length == 2)
    //            {
    //                var color = isSentByCurrentUser ? colorArray[0] : colorArray[1];
    //                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    //            }
    //        }
    //        return Brushes.White;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}