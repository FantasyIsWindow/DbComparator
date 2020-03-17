using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    public class LoadViewVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool vis)
            {
                return vis ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
