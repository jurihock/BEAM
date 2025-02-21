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
    public object Convert(object? value, Type? targetType, object? parameter, CultureInfo culture)
    {
        if (value is int length)
        {
            return Math.Max(0, length - 1); // Since index is zero-based
        }
        return 0;
    }

    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}