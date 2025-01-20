using System;
using ScottPlot;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BEAM.Image.Displayer;
using BEAM.ImageSequence;

namespace BEAM.IMage.Displayer.Scottplot;

public sealed class BitmapPlottable(Sequence sequence) : IPlottable
{
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = new Axes();
    public IEnumerable<LegendItem> LegendItems => Enumerable.Empty<LegendItem>();

    private readonly SequenceImage _image = new(sequence);

    public AxisLimits GetAxisLimits()
    {
        return new AxisLimits(0, sequence.Shape.Width, sequence.Shape.Height - sequence.Shape.Width,
            sequence.Shape.Height);
    }

    public void Render(RenderPack rp)
    {
        // TODO: FIX SCROLLING PROBLEM
        var screenWidth = rp.Canvas.DeviceClipBounds.Width;
        var screenHeight = rp.Canvas.DeviceClipBounds.Height;

        var cropXMin = rp.Plot.Grid.XAxis.Min;
        var cropXMax = rp.Plot.Grid.XAxis.Max;
        var cropYMin = rp.Plot.Grid.YAxis.Min;
        var cropYMax = rp.Plot.Grid.YAxis.Max;

        var bitmap = _image.GetImage((long)cropXMin, (long)cropXMax, sequence.Shape.Height - (long)cropYMax,
            sequence.Shape.Height - (long)cropYMin, screenWidth, screenHeight);

        cropXMin = Math.Clamp(cropXMin, 0, sequence.Shape.Width);
        cropXMax = Math.Clamp(cropXMax, 0, sequence.Shape.Width);
        cropYMin = Math.Clamp(cropYMin, 0, sequence.Shape.Height);
        cropYMax = Math.Clamp(cropYMax, 0, sequence.Shape.Height);

        using SKPaint paint = new()
        {
            FilterQuality = SKFilterQuality.None // WTF
        };

        var dest = Axes.GetPixelRect(new CoordinateRect(cropXMin, cropXMax, cropYMax, cropYMin)).ToSKRect();
        rp.Canvas.DrawBitmap(bitmap, dest, paint);

        /*using SKPaint paint = new()
        {
            FilterQuality = SKFilterQuality.None // WTF
        };
        var screenWidth = rp.Canvas.DeviceClipBounds.Width;
        var screenHeight = rp.Canvas.DeviceClipBounds.Height;
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
        var cropRect = new SKRectI((int)cropXMin, Bitmap.Height - (int)cropYMax - 1, (int)cropXMax + 1,
            Bitmap.Height - (int)cropYMin);
        using var croppedBitmap = new SKBitmap();

        Bitmap.ExtractSubset(croppedBitmap, cropRect);

        // Scale the bitmap
        var scaleX = screenWidth / (cropXMax - cropXMin);
        var scaleY = screenHeight / (cropYMax - cropYMin);
        scaleX = Math.Min(scaleX, 1);
        scaleY = Math.Min(scaleY, 1);

        using var scaledBitmap = croppedBitmap.Resize(
            new SKImageInfo((int)(croppedBitmap.Width * scaleX), (int)(croppedBitmap.Height * scaleY)),
            SKFilterQuality.None);
        SKRect dest = Axes.GetPixelRect(new CoordinateRect(cropXMin - diffX, cropXMin + croppedBitmap.Width - diffX,
            cropYMin + croppedBitmap.Height - diffY, cropYMin - diffY)).ToSKRect();
        rp.Canvas.DrawBitmap(scaledBitmap, dest, paint);*/
    }
}