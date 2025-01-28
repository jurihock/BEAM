using ScottPlot;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using BEAM.Image.Displayer;
using BEAM.ImageSequence;
using ScottPlot.Avalonia;

namespace BEAM.IMage.Displayer.Scottplot;

public sealed class BitmapPlottable(Sequence sequence, AvaPlot avaPlot) : IPlottable
{
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = new Axes();
    public IEnumerable<LegendItem> LegendItems => Enumerable.Empty<LegendItem>();

    private readonly SequenceImage _image = new(sequence, avaPlot);

    public AxisLimits GetAxisLimits()
    {
        //return new AxisLimits(0, sequence.Shape.Width, 0, sequence.Shape.Width);
        return new AxisLimits(0, sequence.Shape.Width, sequence.Shape.Width / 2, -sequence.Shape.Width / 2);
    }

    public void Render(RenderPack rp)
    {
        rp.Plot.Axes.InvertY();

        // min <-> max flipped since inverted Y axis
        var minY = rp.Plot.Grid.YAxis.Max;
        var maxY = rp.Plot.Grid.YAxis.Min;
        _image.Update((long)minY, (long)maxY, rp.Canvas.DeviceClipBounds.Height);

        // drawing the images
        using SKPaint paint = new();
        paint.FilterQuality = SKFilterQuality.None;

        for (var i = 0; i < _image.GetPreviewCount(); i++)
        {
            var preview = _image.GetPreview(i);
            //if (preview.Bitmap is null) continue;

            var coordinateRect = new CoordinateRect()
            {
                Left = 0,
                Right = sequence.Shape.Width,
                Top = preview.YStart,
                Bottom = preview.YEnd,
            };

            var dest = Axes.GetPixelRect(coordinateRect);
            rp.Canvas.DrawBitmap(preview.Bitmap, dest.ToSKRect(), paint);
        }

        /*
        // TODO: FIX PANNING PIXELS
        var screenWidth = rp.Canvas.DeviceClipBounds.Width;
        var screenHeight = rp.Canvas.DeviceClipBounds.Height;

        var cropXMin = rp.Plot.Grid.XAxis.Min;
        var cropXMax = rp.Plot.Grid.XAxis.Max;SKP
        var cropYMin = rp.Plot.Grid.YAxis.Min;
        var cropYMax = rp.Plot.Grid.YAxis.Max;

        var displayWidth = Math.Min(cropXMax - cropXMin, sequence.Shape.Width / (cropXMax - cropXMin) * screenWidth);
        displayWidth = Math.Min(displayWidth, screenWidth);
        var displayHeight = Math.Min(cropYMin - cropYMax, sequence.Shape.Height / (cropYMin - cropYMax) * screenHeight);
        displayHeight = Math.Min(displayHeight, screenHeight);

        using var bitmap = _image.GetImage((long)cropXMin, (long)cropXMax, (long)cropYMax, (long)cropYMin, (int) displayWidth,
            (int) displayHeight);

        cropXMin = Math.Clamp(cropXMin, 0, sequence.Shape.Width);
        cropXMax = Math.Clamp(cropXMax, 0, sequence.Shape.Width);
        cropYMin = Math.Clamp(cropYMin, 0, sequence.Shape.Height);
        cropYMax = Math.Clamp(cropYMax, 0, sequence.Shape.Height);

        if (cropXMin >= cropXMax) return;
        if (cropYMax >= cropYMin) return;

        var diffX = cropXMin - Math.Floor(cropXMin);
        var diffY = cropYMax - Math.Floor(cropYMax);

        using SKPaint paint = new();
        paint.FilterQuality = SKFilterQuality.None; // WTF

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