using System.Collections.ObjectModel;

namespace DbComparator.App.Infrastructure.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void ClearIfNotEmpty<T>(this ObservableCollection<T> collection)
        {
            if (collection != null)
            {
                collection.Clear();
            }
        }
    }
}
