using System.Runtime.InteropServices;
using BEAM.Image.Skia;

namespace BEAM.Tests.Image.Skia;

using SkiaSharp;
using Xunit;
using System;

[Collection("GlobalTests")]
public class SkiaImageExtensionsTests
{
    [Fact]
    public void TypeOf_ReturnsCorrectType_ForSupportedColorTypes()
    {
        Assert.Equal(typeof(byte), SKColorType.Gray8.TypeOf());
        Assert.Equal(typeof(byte), SKColorType.Bgra8888.TypeOf());
        Assert.Equal(typeof(byte), SKColorType.Rgba8888.TypeOf());
        Assert.Equal(typeof(byte), SKColorType.Rgb888x.TypeOf());
    }

    [Fact]
    public void TypeOf_ThrowsNotSupportedException_ForUnsupportedColorType()
    {
        var unsupportedType = (SKColorType)999;
        Assert.Throws<NotSupportedException>(() => unsupportedType.TypeOf());
    }

    [Fact]
    public void SizeOf_ReturnsCorrectSize_ForSupportedColorTypes()
    {
        Assert.Equal(Marshal.SizeOf(typeof(byte)), SKColorType.Gray8.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(byte)), SKColorType.Bgra8888.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(byte)), SKColorType.Rgba8888.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(byte)), SKColorType.Rgb888x.SizeOf());
    }

    [Fact]
    public void UnsafeRead_ReturnsCorrectValue_FromBitmap()
    {
        using var bmp = new SKBitmap(new SKImageInfo(2, 2, SKColorType.Bgra8888));
        var pixels = bmp.GetPixels();
        Marshal.WriteByte(pixels, 0, 255); // B
        Marshal.WriteByte(pixels, 1, 0);   // G
        Marshal.WriteByte(pixels, 2, 0);   // R
        Marshal.WriteByte(pixels, 3, 255); // A

        var value = bmp.UnsafeRead<byte>(0);
        Assert.Equal(255, value);
    }

    [Fact]
    public void CreateValueGetter_ReturnsCorrectValue_ForGivenOffset()
    {
        using var bmp = new SKBitmap(new SKImageInfo(2, 2, SKColorType.Bgra8888));
        var pixels = bmp.GetPixels();
        Marshal.WriteByte(pixels, 0, 255); // B
        Marshal.WriteByte(pixels, 1, 0);   // G
        Marshal.WriteByte(pixels, 2, 0);   // R
        Marshal.WriteByte(pixels, 3, 255); // A

        var getter = bmp.CreateValueGetter<byte>();
        var value = getter(0);

        Assert.Equal(255, value);
    }

    [Fact]
    public void CreateColorChannelDecoder_ReturnsCorrectFunction_ForRgbColorType()
    {
        using var bmp = new SKBitmap(new SKImageInfo(2, 2, SKColorType.Rgba8888));
        var decoder = bmp.CreateColorChannelDecoder();

        Assert.Equal(2, decoder(0)); // R -> B
        Assert.Equal(0, decoder(2)); // B -> R
        Assert.Equal(1, decoder(1)); // G -> G
    }

    [Fact]
    public void CreateColorChannelDecoder_ReturnsIdentityFunction_ForBgrColorType()
    {
        using var bmp = new SKBitmap(new SKImageInfo(2, 2, SKColorType.Bgra8888));
        var decoder = bmp.CreateColorChannelDecoder();

        Assert.Equal(0, decoder(0)); // B -> B
        Assert.Equal(1, decoder(1)); // G -> G
        Assert.Equal(2, decoder(2)); // R -> R
    }
}