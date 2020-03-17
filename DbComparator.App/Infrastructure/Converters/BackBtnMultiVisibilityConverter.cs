using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    class BackBtnMultiVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                if (item is Visibility vis)
                {
                    if (vis == Visibility.Visible)
                    {
                        return Visibility.Visible;
                    }
                }
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
