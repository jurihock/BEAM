namespace BEAM.Tests.Image.Bitmap;

using BEAM.Image.Bitmap;
using Datatypes.Color;
using Xunit;
using System;
using System.Runtime.InteropServices;

public class BgraBitmapTests
{
    [Fact]
    public void Constructor_SetsCorrectDimensions()
    {
        var bitmap = new BgraBitmap(10, 20);

        Assert.Equal(10, bitmap.Width);
        Assert.Equal(20, bitmap.Height);
    }

    [Fact]
    public void Indexer_ReturnsCorrectPixelReference_ByCoordinates()
    {
        var bitmap = new BgraBitmap(10, 10);
        var bgr = new BGR(10, 20, 30);
        var pixel = new BGRA(bgr, 40);
        bitmap[0, 0] = pixel;

        ref var result = ref bitmap[0, 0];

        Assert.Equal(pixel, result);
    }

    [Fact]
    public void Indexer_ReturnsCorrectPixelReference_ByIndex()
    {
        var bitmap = new BgraBitmap(10, 10);
        var bgr = new BGR(10, 20, 30);
        var pixel = new BGRA(bgr, 40);
        bitmap[0] = pixel;

        ref var result = ref bitmap[0];

        Assert.Equal(pixel, result);
    }

    [Fact]
    public void GetPixelSpan_ReturnsCorrectSpanLength()
    {
        var bitmap = new BgraBitmap(10, 10);
        var span = bitmap.GetPixelSpan();

        Assert.Equal(10 * 10 * Marshal.SizeOf<BGRA>(), span.Length);
    }

    [Fact]
    public void GetPixelSpan_ReturnsWritableSpan()
    {
        var bitmap = new BgraBitmap(10, 10);
        var span = bitmap.GetPixelSpan();
        span[0] = 255;

        Assert.Equal(255, span[0]);
    }

    [Fact]
    public void Indexer_ThrowsArgumentOutOfRangeException_WhenCoordinatesAreOutOfRange()
    {
        var bitmap = new BgraBitmap(10, 10);

        Assert.Throws<ArgumentOutOfRangeException>(() => bitmap[-1, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmap[0, -1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmap[10, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmap[0, 10]);
    }

    [Fact]
    public void Indexer_ThrowsArgumentOutOfRangeException_WhenIndexIsOutOfRange()
    {
        var bitmap = new BgraBitmap(10, 10);

        Assert.Throws<ArgumentOutOfRangeException>(() => bitmap[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => bitmap[100]);
    }
}