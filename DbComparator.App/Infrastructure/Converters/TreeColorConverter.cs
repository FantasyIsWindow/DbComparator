using System;
using System.Globalization;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    public class TreeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str != "Tables" && str != "Procedures")
                {
                    return str != "null" ? "Green" : "Red";
                }                
            }
            return "Black";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
