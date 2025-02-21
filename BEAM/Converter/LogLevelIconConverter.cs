using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace BEAM.Converter;

public class LogLevelIconConverter : IValueConverter
{

    /// <summary>
    /// Converts log level strings to corresponding icon paths.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string currentLevel)
        {
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        var img = currentLevel switch
        {
            "INFO" => "../Assets/info.svg",
            "WARNING" => "../Assets/warning.svg",
            "ERROR" => "../Assets/error.svg",
            _ => string.Empty
        };
        return img;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}