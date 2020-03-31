using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool NotNull<T>(this IEnumerable<T> collection)
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

        public static string EnamerableToString<T>(this IEnumerable<T> enumetable) => 
            string.Join(" ", enumetable.ToArray());
    }
}
