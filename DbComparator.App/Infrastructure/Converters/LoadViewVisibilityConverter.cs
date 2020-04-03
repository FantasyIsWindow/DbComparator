using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    public class LoadViewVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Checks the passed value for true, returns the value of the loading window visibility
        /// </summary>
        /// <param name="values">Values</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>The value of the visibility of a window download</returns>
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
