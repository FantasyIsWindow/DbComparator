using DbComparator.App.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    class MessageBoxVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Returns the visibility of the message window, depending on the parameter MbShowDialog
        /// </summary>
        /// <param name="values">Values</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>The visibility of the message window</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MbShowDialog state)
            {
                return state == MbShowDialog.OkCancelState ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
