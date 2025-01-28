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

        for (var i = 0; i < _image.GetRenderedPartsCount(); i++)
        {
            var preview = _image.GetRenderedPart(i);
            if (preview?.Bitmap is null) continue;

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
    }
}