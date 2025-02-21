using System;
using System.Globalization;
using Avalonia.Data.Converters;
using BEAM.Converter;
using Xunit;

public class LengthToMaxConverterTests
{
    [Fact]
    public void Convert_ReturnsMaxIndex_WhenLengthIsPositive()
    {
        // Arrange
        var converter = new LengthToMaxConverter();
        const int length = 5;

        // Act
        var result = converter.Convert(length, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(4, result);
    }

    [Fact]
    public void Convert_ReturnsZero_WhenLengthIsZero()
    {
        // Arrange
        var converter = new LengthToMaxConverter();
        const int length = 0;

        // Act
        var result = converter.Convert(length, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Convert_ReturnsZero_WhenValueIsNotInt()
    {
        // Arrange
        var converter = new LengthToMaxConverter();
        const string value = "Not an integer value";

        // Act
        var result = converter.Convert(value, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        var converter = new LengthToMaxConverter();

        // Act & Assertintint
        Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, null, null, CultureInfo.InvariantCulture));
    }
}