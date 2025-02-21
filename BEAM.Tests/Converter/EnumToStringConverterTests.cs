using System;
using System.Globalization;
using Avalonia.Data.Converters;
using BEAM.Converter;
using BEAM.Image;
using Xunit;

public class EnumToStringConverterTests
{
    [Fact]
    public void Convert_ReturnsStringRepresentation_WhenEnumValueIsValid()
    {
        var converter = new EnumToStringConverter();
        
        const DayOfWeek enumValue = DayOfWeek.Monday;
        var result = converter.Convert(enumValue, null, null, CultureInfo.InvariantCulture);
        Assert.Equal("Monday", result);

        const SequenceType newEnumValue = SequenceType.Envi;
        result = converter.Convert(newEnumValue, null, null, CultureInfo.InvariantCulture);
        Assert.Equal("Envi", result);
    }

    [Fact]
    public void Convert_ThrowsNullReferenceException_WhenValueIsNull()
    {
        var converter = new EnumToStringConverter();

        Assert.Throws<NullReferenceException>(() => converter.Convert(null, null, null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertBack_ReturnsEnumValue_WhenStringRepresentationIsValid()
    {
        var converter = new EnumToStringConverter();
        
        const string stringValue = "Monday";
        var result = converter.ConvertBack(stringValue, typeof(DayOfWeek), null, CultureInfo.InvariantCulture);
        Assert.Equal(DayOfWeek.Monday, result);
        
        const string newStringValue = "Png";
        result = converter.ConvertBack(newStringValue, typeof(SequenceType), null, CultureInfo.InvariantCulture);
        Assert.Equal(SequenceType.Png, result);
    }

    [Fact]
    public void ConvertBack_ReturnsNull_WhenStringRepresentationIsInvalid()
    {
        var converter = new EnumToStringConverter();
        
        const string stringValue = "BadSequenceFormat";
        var result = converter.ConvertBack(stringValue, typeof(SequenceType), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_ReturnsNull_WhenValueIsNull()
    {
        var converter = new EnumToStringConverter();
        
        var result = converter.ConvertBack(null, typeof(DayOfWeek), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }
}