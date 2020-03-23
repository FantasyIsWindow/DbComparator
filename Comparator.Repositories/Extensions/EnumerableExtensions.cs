using System.Collections.Generic;

namespace Comparator.Repositories.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNull<T>(this IEnumerable<T> collection)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
