using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BEAM.Converter;

/// <summary>
/// Converts an enum value to its string representation and vice versa.
/// </summary>
public class EnumToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts an enum value to its string representation.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="culture">The culture to use in the converter (not used).</param>
    /// <returns>The string representation of the enum value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString();
    }

    /// <summary>
    /// Converts a string representation of an enum value back to the enum value.
    /// </summary>
    /// <param name="value">The string representation of the enum value.</param>
    /// <param name="targetType">The type of the enum to convert to.</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="culture">The culture to use in the converter (not used).</param>
    /// <returns>The enum value corresponding to the string representation, or null if the conversion fails.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Enum.TryParse(targetType, value?.ToString(), out var result))
        {
            return result;
        }
        return null;
    }
}