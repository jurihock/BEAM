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

    public void Render(RenderPack rp)
    {
        using SKPaint paint = new()
        {
            FilterQuality = SKFilterQuality.None // WTF
        };

        // Get the visible range
        var cropXMin = rp.Plot.Grid.XAxis.Min;
        var cropXMax = rp.Plot.Grid.XAxis.Max;
        var cropYMin = rp.Plot.Grid.YAxis.Min;
        var cropYMax = rp.Plot.Grid.YAxis.Max;

        // Clamp to bitmap bounds
        cropXMin = Math.Clamp(cropXMin, 0, Bitmap.Width);
        cropXMax = Math.Clamp(cropXMax, 0, Bitmap.Width);
        cropYMin = Math.Clamp(cropYMin, 0, Bitmap.Height);
        cropYMax = Math.Clamp(cropYMax, 0, Bitmap.Height);

        var diffX = cropXMin - Math.Floor(cropXMin);
        var diffY = cropYMin - Math.Floor(cropYMin);

        if (cropXMin >= cropXMax) return;
        if (cropYMin >= cropYMax) return;

        // Crop the bitmap
        var cropRect = new SKRectI((int) cropXMin, Bitmap.Height - (int)cropYMax - 1, (int)cropXMax + 1,
            Bitmap.Height - (int)cropYMin);
        using var croppedBitmap = new SKBitmap();

        Bitmap.ExtractSubset(croppedBitmap, cropRect);

        // Scale the bitmap
        var screenWidth = rp.Canvas.DeviceClipBounds.Width;
        var screenHeight = rp.Canvas.DeviceClipBounds.Height;
        var scaleX = screenWidth / (cropXMax - cropXMin);
        var scaleY = screenHeight / (cropYMax - cropYMin);
        scaleX = Math.Min(scaleX, 1);
        scaleY = Math.Min(scaleY, 1);

        using var scaledBitmap = croppedBitmap.Resize(
            new SKImageInfo((int)(croppedBitmap.Width * scaleX), (int)(croppedBitmap.Height * scaleY)),
            SKFilterQuality.None);

        SKRect dest = Axes.GetPixelRect(new CoordinateRect(cropXMin - diffX, cropXMin + croppedBitmap.Width - diffX, cropYMin + croppedBitmap.Height - diffY, cropYMin - diffY)).ToSKRect();
        rp.Canvas.DrawBitmap(scaledBitmap, dest, paint);
    }
}