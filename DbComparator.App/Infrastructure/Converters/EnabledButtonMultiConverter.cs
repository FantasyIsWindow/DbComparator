using DbComparator.App.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    public class EnabledButtonMultiConverter : IMultiValueConverter
    {
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
