using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Extensions
{
    public static class IListExtensions
    {
        /// <summary>
        /// Retrieves a random object from a generic list.
        /// </summary>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) return default(T);

            int indexToGet = RandomProvider.Next(0, list.Count - 1);

            T itemToGet = list[indexToGet];

            return itemToGet;
        }

        /// <summary>
        /// Removes a random object from a generic list.
        /// </summary>
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) return default(T);

            int indexToRemove = RandomProvider.Next(0, list.Count - 1);

            T itemToRemove = list[indexToRemove];
            list.RemoveAt(indexToRemove);

            return itemToRemove;
        }
    }
}
