using System;
using System.Collections.Generic;

namespace BEAM.Image.Minimap.Utility;

public static class HelperMethods
{
    public static List<TK> ReplaceEveryEntry<T, TK>(this IEnumerable<T> inputList, Func<T, TK> conversion)
    {
        List<TK> output = new List<TK>();
        foreach (var element in inputList)
        {
            output.Add(conversion(element));
        }

        return output;
    }
}