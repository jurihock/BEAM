using System;
using ScottPlot;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEAM.IMage.Displayer.Scottplot;

public sealed class BitmapPlottable : IPlottable
{
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = new Axes();
    public IEnumerable<LegendItem> LegendItems => Enumerable.Empty<LegendItem>();

    public SKBitmap Bitmap { get; set; }

    public BitmapPlottable(SKBitmap bitmap)
    {
        Bitmap = bitmap;
    }

    public AxisLimits GetAxisLimits()
    {
        return new AxisLimits(0, Bitmap.Width, Bitmap.Height, 0);
    }

    private int _i = 0;

    public void Render(RenderPack rp)
    {
        using SKPaint paint = new()
        {
            FilterQuality = SKFilterQuality.None // WTF
        };

        /*var visibleWidth = rp.Plot.Grid.XAxis.Max - rp.Plot.Grid.XAxis.Min;
        var visibleHeight = rp.Plot.Grid.YAxis.Max - rp.Plot.Grid.YAxis.Min;
        var scaleX = rp.Canvas.DeviceClipBounds.Width / visibleWidth;
        var scaleY = rp.Canvas.DeviceClipBounds.Height / visibleHeight;
        var bitmapScaleX = Bitmap.Width / visibleWidth;
        var bitmapScaleY = Bitmap.Height / visibleHeight;

        var finalScaleX = double.Min(scaleX, bitmapScaleX);
        var finalScaleY = double.Min(scaleY, bitmapScaleY);
        Console.WriteLine($"{finalScaleX}x{finalScaleY}");

        var extracted =
            new SKBitmap(new SKImageInfo((int)visibleWidth, (int)visibleHeight, Bitmap.ColorType, Bitmap.AlphaType));
        Bitmap.ExtractSubset(extracted, new SKRectI((int)rp.Plot.Grid.XAxis.Min, (int)rp.Plot.Grid.YAxis.Max,
            (int)rp.Plot.Grid.XAxis.Max, (int)rp.Plot.Grid.YAxis.Min));

        var bt = extracted.Resize(new SKImageInfo((int)(Bitmap.Width * finalScaleX), (int)(Bitmap.Height * finalScaleY)),
            SKFilterQuality.None);
        SKRect dest = Axes.GetPixelRect(new CoordinateRect(0, Bitmap.Width, Bitmap.Height, 0)).ToSKRect();
        rp.Canvas.DrawBitmap(bt, dest, paint);
        */
        // Get the visible range
        var cropXMin = rp.Plot.Grid.XAxis.Min;
        var cropXMax = rp.Plot.Grid.XAxis.Max;
        var cropYMin = rp.Plot.Grid.YAxis.Min;
        var cropYMax = rp.Plot.Grid.YAxis.Max;

        var diffX = cropXMin - Math.Floor(cropXMin);
        var diffY = cropYMin - Math.Floor(cropYMin);

        // Clamp to bitmap bounds
        cropXMin = Math.Clamp(cropXMin, 0, Bitmap.Width);
        cropXMax = Math.Clamp(cropXMax, 0, Bitmap.Width);
        cropYMin = Math.Clamp(cropYMin, 0, Bitmap.Height);
        cropYMax = Math.Clamp(cropYMax, 0, Bitmap.Height);

        if (cropXMin >= cropXMax) return;
        if (cropYMin >= cropYMax) return;

        // Crop the bitmap
        var cropRect = new SKRectI((int) cropXMin, Bitmap.Height - (int)cropYMax - 1, (int)cropXMax + 1,
            Bitmap.Height - (int)cropYMin);
        using var croppedBitmap = new SKBitmap();

        Bitmap.ExtractSubset(croppedBitmap, cropRect);

// Step 4: Scale the bitmap
        /*int screenWidth = (int)rp.Canvas.DeviceClipBounds.Width;
        int screenHeight = (int)rp.Canvas.DeviceClipBounds.Height;
        float scaleX = (float)screenWidth / (cropXMax - cropXMin);
        float scaleY = (float)screenHeight / (cropYMax - cropYMin);

        using var scaledBitmap = croppedBitmap.Resize(
            new SKImageInfo((int)(croppedBitmap.Width * scaleX), (int)(croppedBitmap.Height * scaleY)),
            SKFilterQuality.High);*/

// Step 5: Render the bitmap

        SKRect dest = Axes.GetPixelRect(new CoordinateRect(cropXMin - diffX, cropXMin + croppedBitmap.Width - diffX, cropYMin + croppedBitmap.Height - diffY, cropYMin - diffY)).ToSKRect();
        rp.Canvas.DrawBitmap(croppedBitmap, dest, paint);
    }
}