﻿using DbComparator.App.Infrastructure.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    class MessageBoxVisibilityConverter : IValueConverter
    {
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
