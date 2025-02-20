// (c) Paul Stier, 2025
using System.Collections.Generic;
using ScottPlot;
using SkiaSharp;

namespace BEAM.Image.Displayer.ScottPlot;

public class CheckerboardPlottable(bool darkMode = false) : IPlottable
{
    private readonly AssetImage _assetImage = darkMode
        ? new AssetImage("avares://BEAM/Assets/Images/CheckerboardDark.png")
        : new AssetImage("avares://BEAM/Assets/Images/CheckerboardLight.png");

    public AxisLimits GetAxisLimits()
    {
        return new AxisLimits(0, _assetImage.Shape.Width, 0, _assetImage.Shape.Height);
    }

    public void Render(RenderPack rp)
    {
        var minX = rp.Plot.Grid.XAxis.Min;
        var maxX = rp.Plot.Grid.XAxis.Max;
        var minY = rp.Plot.Grid.YAxis.Min;
        var maxY = rp.Plot.Grid.YAxis.Max;

        var origin = Axes.GetPixelRect(new CoordinateRect(0, 100, 0, 100));

        var bmp = _assetImage.GetBitmap();
        var translation = SKMatrix.CreateTranslation(origin.Left, -origin.Top);

        var paint = new SKPaint()
        {
            FilterQuality = SKFilterQuality.None,
            Shader = SKShader.CreateBitmap(bmp, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat,
                translation)
        };

        var coordRect = new CoordinateRect(minX, maxX, minY, maxY);
        var dest = Axes.GetPixelRect(coordRect);
        rp.Canvas.DrawRect(dest.ToSKRect(), paint);
    }

    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = new Axes();
    public IEnumerable<LegendItem> LegendItems => [];
}