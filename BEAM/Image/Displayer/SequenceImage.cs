// (c) Paul Stier, 2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BEAM.Image.Bitmap;
using BEAM.ImageSequence;
using BEAM.Renderer;
using ScottPlot.Avalonia;
using SkiaSharp;

namespace BEAM.Image.Displayer;

public class SequenceImage
{
    private static readonly SKPaint Paint = new SKPaint() { FilterQuality = SKFilterQuality.None };

    public class SectionPreview(
        long yStart,
        SequenceImage seqImg)
    {
        public SKBitmap? Bitmap { get; private set; }

        public long YStart { get; set; } = yStart;
        public long YEnd { get; private set; }
        public double Scale { get; private set; }

        private CancellationTokenSource? _cancellationToken;

        public void Render(Sequence sequence, double resolutionScale, long yRange, AvaPlot plot, bool scaled)
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();

            Scale = resolutionScale;
            var width = sequence.Shape.Width;
            var height = yRange;
            YEnd = YStart + yRange;

            Task.Run(() =>
            {
                if (_cancellationToken.Token.IsCancellationRequested) return;
                if (scaled && Bitmap is not null)
                {
                    /*var tmp = CreateTempBitmap(1, 1, SKColors.Gray.WithAlpha(50));

                    var infoBmp = new SKBitmap(new SKImageInfo(Bitmap.Width, Bitmap.Height));
                    using var canvas = new SKCanvas(infoBmp);
                    canvas.DrawBitmap(Bitmap, new SKPoint(0, 0), Paint);
                    canvas.DrawBitmap(tmp, new SKRectI(0, 0, Bitmap.Width, Bitmap.Height), Paint);

                    if(_cancellationToken.Token.IsCancellationRequested) return;
                    Bitmap = infoBmp;*/
                }
                else
                {
                    Bitmap = CreateTempBitmap(1, 1, SKColors.Gray);
                }

                // calculate width and height based sequence.Shape.Width and yRange
                var bmp = seqImg.GetImage(0, sequence.Shape.Width,
                    YStart, YStart + yRange,
                    (int)(width * resolutionScale), (int)(height * resolutionScale),
                    _cancellationToken);

                Bitmap = bmp;
                plot.Refresh();
            }, _cancellationToken.Token);
        }
    }

    private SectionPreview[] _sectionPreviews = new SectionPreview[11];
    public SectionPreview GetPreview(int index) => _sectionPreviews[index];
    public int GetPreviewCount() => _sectionPreviews.Length;

    private int _GetMaxImageIndex()
    {
        var max = _sectionPreviews[0].YStart;
        var idx = 0;

        for (var i = 1; i < _sectionPreviews.Length; i++)
        {
            var pos = _sectionPreviews[i].YStart;
            if (pos <= max) continue;

            max = pos;
            idx = i;
        }

        return idx;
    }

    private int _GetMinImageIndex()
    {
        var min = _sectionPreviews[0].YStart;
        var idx = 0;

        for (var i = 1; i < _sectionPreviews.Length; i++)
        {
            var pos = _sectionPreviews[i].YStart;
            if (pos >= min) continue;

            min = pos;
            idx = i;
        }

        return idx;
    }

    private List<int> _GetVisibleImageIndexes(long minY, long maxY)
    {
        List<int> data = [];
        for (var i = 0; i < _sectionPreviews.Length; i++)
        {
            var preview = _sectionPreviews[i];
            if (preview.YStart >= minY && preview.YStart <= maxY || preview.YEnd >= minY && preview.YEnd <= maxY || preview.YStart <= minY && preview.YEnd >= maxY)
                data.Add(i);
        }

        return data;
    }

    private (long min, long max) _GetPreviewYRange()
    {
        var min = _sectionPreviews[0].YStart;
        var max = _sectionPreviews[0].YEnd;

        for (var i = 1; i < _sectionPreviews.Length; i++)
        {
            var pos = _sectionPreviews[i].YStart;
            if (pos < min) min = pos;

            var maxPos = _sectionPreviews[i].YEnd;
            if (maxPos > max) max = maxPos;
        }

        return (min, max);
    }

    private readonly Sequence _sequence;
    private readonly AvaPlot _avaPlot;

    public SequenceImage(Sequence sequence, AvaPlot avaPlot)
    {
        _avaPlot = avaPlot;
        _sequence = sequence;
        const int initialHeight = 1000;
        for (var i = 0; i < _sectionPreviews.Length; i++)
        {
            _sectionPreviews[i] = new SectionPreview(i * initialHeight, this);
            _sectionPreviews[i].Render(_sequence, 0.25, initialHeight, avaPlot, false);
        }
    }

    private double? lastScale;
    private long? lastMinY, lastMaxY;

    public void Update(long minY, long maxY, int canvasHeight)
    {
        // TODO: fix zooming, scrolling when scaled
        var scale = Math.Min((double)canvasHeight / (maxY - minY), 1);

        // make 4 scales
        scale *= 4;
        scale = Math.Round(scale);
        scale /= 4;
        scale = Math.Max(0.1, scale);
        Console.WriteLine($"{scale}");

        if (lastScale is null || lastMinY is null || lastMaxY is null)
        {
            lastScale = scale;
            lastMinY = minY;
            lastMaxY = maxY;
            return;
        }

        var renderedRange = _GetPreviewYRange();
        var visibleRangeMid = (maxY - minY) / 2 + minY;
        var positionInRenderedRange =
            (double)(visibleRangeMid - renderedRange.min) / (renderedRange.max - renderedRange.min);

        // scrolled
        if (minY - lastMinY == maxY - lastMaxY || minY - lastMinY + 1 == maxY - lastMaxY || minY - lastMinY - 1 == maxY - lastMaxY)
        {
            // preload down
            if (positionInRenderedRange > 0.66 && renderedRange.max < _sequence.Shape.Height)
            {
                var preview = _sectionPreviews[_GetMinImageIndex()];
                var range = Math.Min(maxY - minY, _sequence.Shape.Height - preview.YStart);

                preview.YStart = renderedRange.max;
                preview.Render(_sequence, scale, range, _avaPlot, false);
            }
            else if (positionInRenderedRange < 0.33 && renderedRange.min > 0)
            {
                var preview = _sectionPreviews[_GetMaxImageIndex()];
                var height = Math.Min(_sequence.Shape.Height - renderedRange.min, maxY - minY);
                preview.YStart = Math.Max(0, preview.YStart - height);

                preview.Render(_sequence, scale, height, _avaPlot, false);
            }
        }
        // zoomed
        // TODO do
        /*else if (lastScale < scale)
        {
            var visibleImagesIdx = _GetVisibleImageIndexes(minY, maxY);
            foreach (var preview in visibleImagesIdx.Select(idx => _sectionPreviews[idx]))
            {
                preview.Render(_sequence, scale, preview.YEnd - preview.YStart, _avaPlot, true);
            }
        }*/

        lastMinY = minY;
        lastMaxY = maxY;
        lastScale = scale;
    }

    public SKBitmap GetImage(long startX, long endX, long startLine, long endLine, int width, int height,
        CancellationTokenSource? tokenSource = null)
    {
        // TODO: change
        SequenceRenderer renderer = new ChannelMapRenderer(0, 255, 2, 1, 0);
        /*
        renderer = new HeatMapRendererRB(0, 1, 1);
        */

        startX = Math.Clamp(startX, 0, _sequence.Shape.Width);
        endX = Math.Clamp(endX, 0, _sequence.Shape.Width);
        startLine = Math.Clamp(startLine, 0, _sequence.Shape.Height);
        endLine = Math.Clamp(endLine, 0, _sequence.Shape.Height);

        width = (int)Math.Clamp(width, 0, endX - startX);
        height = (int)Math.Clamp(height, 0, endLine - startLine);

        BgraBitmap bitmap = new(width, height);

        Parallel.For(0, height, j =>
        {
            var line = startLine + j * (endLine - startLine) / height;

            var xs = new long[width];
            for (var i = 0; i < width; i++)
            {
                xs[i] = startX + i * (endX - startX) / width;
            }

            var data = renderer.RenderPixels(_sequence, xs, line, tokenSource);

            var span = bitmap.GetPixelSpan();
            var pixels = MemoryMarshal.Cast<byte, BGRA>(span);

            for (var i = 0; i < width; i++)
            {
                if (tokenSource?.IsCancellationRequested ?? false) return;
                pixels[j * width + i] = new BGRA()
                {
                    R = data[i, 1],
                    G = data[i, 2],
                    B = data[i, 3],
                    A = data[i, 0]
                };
            }
        });

        return bitmap;
    }

    public static SKBitmap CreateTempBitmap(int width, int height, SKColor color)
    {
        var bitmap = new SKBitmap(new SKImageInfo(width, height));
        var canvas = new SKCanvas(bitmap);
        canvas.Clear(color);
        return bitmap;
    }
}