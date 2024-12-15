using ScottPlot;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

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

    SKRect dest = Axes.GetPixelRect(new CoordinateRect(0, Bitmap.Width, Bitmap.Height, 0)).ToSKRect();

    rp.Canvas.DrawBitmap(Bitmap, dest, paint);
  }
}
