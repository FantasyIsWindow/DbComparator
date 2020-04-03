using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    public class ViewsVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// To check passed value is null, it returns a visibility indicator Views
        /// </summary>
        /// <param name="values">Values</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>A visibility indicator Views</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
