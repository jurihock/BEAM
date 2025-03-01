namespace BEAM.Converter;

using Avalonia.Data.Converters;
using System;
using System.Globalization;

/// <summary>
/// Converts the length of an array to a maximum value the index for this array can have.
/// MaxIndex = array.Length - 1
/// </summary>
public class LengthToMaxConverter : IValueConverter
{
    /// <summary>
    /// Converts the length of an array to the maximum index addressable (max - 1).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (value is int length)
        {
            return Math.Max(0, length - 1); // Since index is zero-based
        }
        return 0;
    }

    /// <summary>
    /// This method is never required.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}