using DbComparator.App.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    public class ChoiseDbFromScriptCreatorTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Passage model)
            {
                return $"({model.Info.DbType}) {model.Info.DbName}";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
