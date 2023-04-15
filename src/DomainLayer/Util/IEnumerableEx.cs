using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Extension
{
    public static class IEnumerableEx
    {
        public static T? Top1OrDefault<T>(this IEnumerable<T> source) where T : class?
        {
            foreach (var item in source)
                return item;

            return null;
        }

        public static T? Top1OrDefault<T>(this IEnumerable<T> source, Func<T?, bool> predicate) where T : class?
        {
            foreach (var item in source)
            {
                if (predicate(item))
                    return item;
            }

            return null;
        }

        public static void Invoke<T>(this IEnumerable<T> source, Action<T> action) where T : class?
        {
            foreach (var item in source)
                action(item);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Func<T?, bool> predicate) where T : class?
        {
            return source
                .Select((a, i) => new { Content = a, Index = i })
                .Top1OrDefault(item => predicate(item?.Content))?.Index ?? -1;
        }
    }
}
