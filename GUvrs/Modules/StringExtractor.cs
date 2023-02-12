using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUvrs.Modules;

public static class StringExtractor
{
    public static string Extract(this string value, string string1, string string2)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var index1 = value.IndexOf(string1, StringComparison.Ordinal) + string1.Length;
        var step = value.Substring(index1);
        var length = step.IndexOf(string2, StringComparison.Ordinal);

        return value.Substring(index1, length);
    }
}
