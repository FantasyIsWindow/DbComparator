using System;
using System.Globalization;
using System.Windows.Data;

namespace DbComparator.App.Infrastructure.Converters
{
    public class SharedCheckboxStateMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool? returnedState = null;

            foreach (var value in values)
            {
                var state = value as bool?;

                if (state == true)
                {
                    if (returnedState == false)
                    {
                        return null;
                    }
                    returnedState = true;
                }
                else
                {
                    if (returnedState == true)
                    {
                        return null;
                    }
                    returnedState = false;
                }
            }
            return returnedState;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var state = value as bool?;
            object[] returnedStates = new object[targetTypes.Length];

            if(state == true)
            {
                FillArr(returnedStates, true);
            }
            else
            {
                FillArr(returnedStates, false);
            }
            return returnedStates;
        }

        private void FillArr(object[] arr, bool state)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = state;
            }
        }
    }
}
