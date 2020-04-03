using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks the collection for null or zero
        /// </summary>
        /// <typeparam name="T">A generalized class</typeparam>
        /// <param name="collection">Check the collection</param>
        /// <returns>Returns the verification result</returns>
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

        /// <summary>
        /// Converting a collection to a string
        /// </summary>
        /// <typeparam name="T">A generalized class</typeparam>
        /// <param name="enumetable">Convertible collection</param>
        /// <returns>Returns a string after conversion</returns>
        public static string EnumerableToString<T>(this IEnumerable<T> enumetable) => 
            string.Join(" ", enumetable.ToArray());
    }
}
