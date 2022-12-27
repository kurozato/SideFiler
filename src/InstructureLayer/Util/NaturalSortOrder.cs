using BlackSugar.Model;
using System.Runtime.InteropServices;
using System.Security;

namespace BlackSugar.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);
    }

    public sealed class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string? a, string? b) => NativeMethods.StrCmpLogicalW(a, b);
    }


    public static class NaturalSortOrderEx
    {
        public static IOrderedEnumerable<T> NaturallyOrderBy<T>(this IEnumerable<T> Source, Func<T, string> KeySelector)
        {
            return Source.OrderBy(KeySelector, new NaturalStringComparer());
        }

        public static IOrderedEnumerable<T> NaturallyOrderByDescending<T>(this IEnumerable<T> Source, Func<T, string> KeySelector)
        {
            return Source.OrderByDescending(KeySelector, new NaturalStringComparer());
        }

        public static IOrderedEnumerable<T> NaturallyThenBy<T>(this IOrderedEnumerable<T> Source, Func<T, string> KeySelector)
        {
            return Source.ThenBy(KeySelector, new NaturalStringComparer());
        }

        public static IOrderedEnumerable<T> NaturallyThenByDescending<T>(this IOrderedEnumerable<T> Source, Func<T, string> KeySelector)
        {
            return Source.ThenByDescending(KeySelector, new NaturalStringComparer());
        }

    }

}
