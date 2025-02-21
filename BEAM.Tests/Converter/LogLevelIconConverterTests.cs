using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using BEAM.Converter;
using Xunit;

public class LogLevelIconConverterTests
{
    [Fact]
    public void Convert_ReturnsInfoIconPath_WhenLogLevelIsInfo()
    {
        var converter = new LogLevelIconConverter();
        const string logLevel = "INFO";

        var result = converter.Convert(logLevel, null, null, CultureInfo.InvariantCulture);

        Assert.Equal("../Assets/info.svg", result);
    }

    [Fact]
    public void Convert_ReturnsWarningIconPath_WhenLogLevelIsWarning()
    {
        var converter = new LogLevelIconConverter();
        const string logLevel = "WARNING";

        var result = converter.Convert(logLevel, null, null, CultureInfo.InvariantCulture);

        Assert.Equal("../Assets/warning.svg", result);
    }

    [Fact]
    public void Convert_ReturnsErrorIconPath_WhenLogLevelIsError()
    {
        var converter = new LogLevelIconConverter();
        const string logLevel = "ERROR";

        var result = converter.Convert(logLevel, null, null, CultureInfo.InvariantCulture);

        Assert.Equal("../Assets/error.svg", result);
    }

    [Fact]
    public void Convert_ReturnsEmptyString_WhenLogLevelIsUnknown()
    {
        var converter = new LogLevelIconConverter();
        const string logLevel = "UNKNOWN";

        var result = converter.Convert(logLevel, null, null, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Convert_ReturnsBindingNotification_WhenValueIsNotString()
    {
        var converter = new LogLevelIconConverter();
        const int logLevel = 123;

        var result = converter.Convert(logLevel, null, null, CultureInfo.InvariantCulture);

        Assert.IsType<BindingNotification>(result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new LogLevelIconConverter();

        Assert.Throws<NotSupportedException>(() => converter.ConvertBack(null, null, null, CultureInfo.InvariantCulture));
    }
}