using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.KendoUI.Extensions
{
    public static class IListExtensions
    {
        public static void ReplaceAt<T>(this IList<T> items, int indexPosition, T newItem)
        {
            items.RemoveAt(indexPosition);
            items.Insert(indexPosition, newItem);
        }
        public static void Replace<T>(this IList<T> items, T item, T newItem)
        {
            var index = items.IndexOf(item);
            if (index >= 0) items.ReplaceAt(index, newItem);
        }
    }
}
