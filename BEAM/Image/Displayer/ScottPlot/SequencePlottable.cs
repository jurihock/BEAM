using System;
using ScottPlot;
using SkiaSharp;
using System.Collections.Generic;
using BEAM.Image.Displayer;
using BEAM.ImageSequence;
using BEAM.Renderer;

namespace BEAM.Image.Displayer.Scottplot;

/// <summary>
/// Plottable for rendering a sequence to a ScottPlot plot.
/// </summary>
/// <param name="sequence">The sequence to draw</param>
/// <param name="renderer">The renderer used to draw the sequence</param>
/// <param name="startLine">The starting line number to draw the sequence from</param>
public sealed class SequencePlottable(ISequence sequence, SequenceRenderer renderer, long startLine = 0) : IPlottable
{
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = new Axes();
    public IEnumerable<LegendItem> LegendItems => [];

    public readonly SequenceImage SequenceImage = new(sequence, startLine, renderer);

    public AxisLimits GetAxisLimits()
    {
        //return new AxisLimits(0, sequence.Shape.Width, 0, sequence.Shape.Width);
        return new AxisLimits(0, sequence.Shape.Width, Math.Floor(sequence.Shape.Width / 2.0) + startLine,
            -Math.Floor(sequence.Shape.Width / 2.0) + startLine);
    }

    /// <summary>
    /// Updated the selected renderer to draw the sequence with.
    /// </summary>
    /// <param name="renderer"></param>
    public void ChangeRenderer(SequenceRenderer renderer)
    {
        SequenceImage.Renderer = renderer;
    }

    public void Render(RenderPack rp)
    {
        // Drawing offset for transformed sequence
        var xOffset = -0.5;
        var yOffset = -0.5;
        if (sequence is TransformedSequence transformedSequence)
        {
            xOffset += transformedSequence.DrawOffsetX;
            yOffset += transformedSequence.DrawOffsetY;
        }

        rp.Plot.Axes.InvertY();

        // min <-> max flipped since inverted Y axis
        var minY = rp.Plot.Grid.YAxis.Max;
        var maxY = rp.Plot.Grid.YAxis.Min;
        SequenceImage.Update((long)minY, (long)maxY, rp.Canvas.DeviceClipBounds.Height);

        // drawing the images
        using SKPaint paint = new();
        paint.FilterQuality = SKFilterQuality.None;

        for (var i = 0; i < SequenceImage.GetRenderedPartsCount(); i++)
        {
            var preview = SequenceImage.GetRenderedPart(i);
            if (preview?.Bitmap is null) continue;

            // positions the rendered images
            var coordinateRect = new CoordinateRect()
            {
                Left = xOffset,
                Right = sequence.Shape.Width + xOffset,
                Top = preview.YStart + yOffset,
                Bottom = preview.YEnd + yOffset
            };
            var dest = Axes.GetPixelRect(coordinateRect);

            // draws the images
            rp.Canvas.DrawBitmap(preview.Bitmap, dest.ToSKRect(), paint);
        }
    }
}