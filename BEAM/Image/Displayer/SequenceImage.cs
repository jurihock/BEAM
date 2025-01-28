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

// TODO: Disposable
public class SequenceImage
{
    private static readonly long SectionHeight = 1000;
    private static readonly int MinPreloadedSections = 20;

    private static readonly SKPaint Paint = new SKPaint() { FilterQuality = SKFilterQuality.None };

    public class SectionPreview(
        long yStart,
        SequenceImage seqImg) : IDisposable
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
                    var tmp = CreateTempBitmap(1, 1, SKColors.Gray.WithAlpha(50));

                    var infoBmp = new SKBitmap(new SKImageInfo(Bitmap.Width, Bitmap.Height));
                    using var canvas = new SKCanvas(infoBmp);
                    canvas.DrawBitmap(Bitmap, new SKPoint(0, 0), Paint);
                    canvas.DrawBitmap(tmp, new SKRectI(0, 0, Bitmap.Width, Bitmap.Height), Paint);

                    if(_cancellationToken.Token.IsCancellationRequested) return;
                    Bitmap = infoBmp;
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

        public void Dispose()
        {
            _cancellationToken?.Dispose();
            Bitmap?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    private List<SectionPreview> _sectionPreviews = [];
    public SectionPreview GetPreview(int index) => _sectionPreviews[index];
    public int GetPreviewCount() => _sectionPreviews.Count;

    private (long min, long max) _GetPreviewYRange()
    {
        return (_sectionPreviews[0].YStart, _sectionPreviews[^1].YEnd);
    }

    private readonly Sequence _sequence;
    private readonly AvaPlot _avaPlot;

    public SequenceImage(Sequence sequence, AvaPlot avaPlot)
    {
        _avaPlot = avaPlot;
        _sequence = sequence;
        for (var i = 0; i < MinPreloadedSections; i++)
        {
            _sectionPreviews.Add(new SectionPreview(i * SectionHeight, this));
            _sectionPreviews[i].Render(_sequence, 0.25, SectionHeight, avaPlot, false);
        }
    }

    public void Update(long minY, long maxY, int canvasHeight)
    {
        var scale = Math.Min((double)canvasHeight / (maxY - minY), 1);

        // make 4 scales
        scale *= 4;
        scale = Math.Round(scale);
        scale /= 4;
        scale = Math.Max(0.25, scale);

        var renderedRange = _GetPreviewYRange();
        var visibleRangeMid = (maxY - minY) / 2 + minY;
        var positionInRenderedRange =
            (double)(visibleRangeMid - renderedRange.min) / (renderedRange.max - renderedRange.min);

        // preload down
        if ((positionInRenderedRange > 0.66 || renderedRange.max < maxY) && renderedRange.max < _sequence.Shape.Height)
        {
            while (GetPreviewCount() > MinPreloadedSections && _sectionPreviews[0].YEnd < minY)
            {
                _sectionPreviews[0].Dispose();
                _sectionPreviews.RemoveAt(0);
            }

            // add image down below
            var preview = new SectionPreview(renderedRange.max, this);
            _sectionPreviews.Add(preview);
            var range = Math.Min(SectionHeight, _sequence.Shape.Height - renderedRange.max);
            preview.Render(_sequence, 0.25, range, _avaPlot, false);
        }

        // preload up
        if ((positionInRenderedRange < 0.33 || renderedRange.min > minY) && renderedRange.min > 0)
        {
            while (GetPreviewCount() > MinPreloadedSections && _sectionPreviews[^1].YStart > maxY)
            {
                _sectionPreviews[^1].Dispose();
                _sectionPreviews.RemoveAt(_sectionPreviews.Count - 1);
            }

            // add an image above
            var start = Math.Max(renderedRange.min - SectionHeight, 0);
            var preview = new SectionPreview(start, this);
            _sectionPreviews.Insert(0, preview);

            var range = Math.Min(SectionHeight, renderedRange.min);
            preview.Render(_sequence, 0.25, range, _avaPlot, false);
        }

        // account for zooming
        foreach (var preview in _sectionPreviews.Where((preview) =>
                     (preview.YStart >= minY && preview.YStart <= maxY ||
                      preview.YEnd >= minY && preview.YEnd <= maxY ||
                      preview.YStart <= minY && preview.YEnd >= maxY) && preview.Scale < scale))
        {
            preview.Render(_sequence, scale, preview.YEnd - preview.YStart, _avaPlot, true);
        }
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