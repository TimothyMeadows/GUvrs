using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUvrs.Modules
{
    public static class StringExtractor
    {
        public static string Extract(this string value, string string1, string string2)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var index = value.LastIndexOf(string1, StringComparison.Ordinal) + string1.Length + 1;
            var length = value.IndexOf(string2, StringComparison.Ordinal) - index;

            return value.Substring(index, length);
        }
    }
}
