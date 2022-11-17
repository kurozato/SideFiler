
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BlackSugar.Extension
{
    public static class StringEx
    {
        public static string ToUpperCamel(this string source)
        {
            if (string.IsNullOrEmpty(source)) return source;
            
            return string.Concat(source.Substring(0, 1).ToUpper(), source.AsSpan(1));
        }

        public static TEnum TryParse<TEnum>(this string? value) where TEnum : struct
        {
            TEnum eValue = default(TEnum);
            if (value == null) return eValue;

            var target = value.ToUpperCamel();

            if (Enum.IsDefined(typeof(TEnum), target))
                Enum.TryParse<TEnum>(target, out eValue);

            return eValue;
        }
    }
}
