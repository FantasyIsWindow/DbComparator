using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DbComparator.App.Infrastructure.Extensions
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// Collection to observableCollection converter
        /// </summary>
        /// <typeparam name="T">A generalized class</typeparam>
        /// <param name="collection">Convertible collection</param>
        /// <returns>ObservablCollection</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection) =>
            new ObservableCollection<T>(collection);       
    }
}
