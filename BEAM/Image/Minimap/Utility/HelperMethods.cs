using System;
using System.Collections.Generic;

namespace BEAM.Image.Minimap.Utility;
/// <summary>
/// This class provides general utility methods.
/// </summary>
public static class HelperMethods
{
    /// <summary>
    /// Replaces every entry in a list with a new entry, returning a list of the new entries without changing the original list.
    /// </summary>
    /// <param name="inputList">The list on whose elements the conversion will be performed</param>
    /// <param name="conversion">A function converting the elements.</param>
    /// <typeparam name="T">The type of the input elements.</typeparam>
    /// <typeparam name="TK">The type of the output elements.</typeparam>
    /// <returns>A list of the resulting objects.</returns>
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