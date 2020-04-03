using DbComparator.App.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    public class EnabledBtnMultiConverter : IMultiValueConverter
    {
        /// <summary>
        /// Checks the passed value for null and returns confirmation that the buttons are available
        /// </summary>
        /// <param name="values">Values</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>Confirmation of button availability</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                var db = item as DbInfo;

                if (db == null || !db.IsConnect)
                {
                    return false;
                }
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
