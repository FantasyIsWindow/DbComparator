using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DbComparator.App.Infrastructure.Extensions
{
    public static class EnumerableExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection) =>
            new ObservableCollection<T>(collection);        
    }
}
