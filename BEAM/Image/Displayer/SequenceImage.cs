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

/// <summary>
/// A class managing the actual rendering of a sequence to multiple bitmaps.
/// </summary>
public class SequenceImage : IDisposable
{
    /// <summary>
    /// The height (in lines) of the sub images.
    /// </summary>
    private const long SectionHeight = 1000;

    /// <summary>
    /// The minimum count of loaded sequence excerpts.
    /// </summary>
    private int MinPreloadedSections = 20;

    /// <summary>
    /// A class representing a rendered part of a sequence, complete with rendering functionality.
    /// </summary>
    /// <param name="sequence">The sequence to render from</param>
    /// <param name="seqImg">The SequenceImage class which manages this object</param>
    /// <param name="yStart">The position where to place this sequence part inside a sequence</param>
    public class SequencePart(Sequence sequence, SequenceImage seqImg, long yStart) : IDisposable
    {
        private static readonly SKPaint Paint = new() { FilterQuality = SKFilterQuality.None };

        /// <summary>
        /// The rendered Bitmap of the sequence.
        /// </summary>
        public SKBitmap? Bitmap { get; private set; }

        private SKBitmap? _bitmap;

        /// <summary>
        /// The start of the excerpt of the rendered sequence.
        /// </summary>
        public long YStart { get; } = yStart;

        /// <summary>
        /// The end of the excerpt of the rendered sequence.
        /// </summary>
        public long YEnd { get; private set; }

        /// <summary>
        /// The used scale when rendering the corresponding part of the sequence.
        /// </summary>
        public double Scale { get; private set; }

        private CancellationTokenSource? _cancellationToken;

        /// <summary>
        /// Renders the part of the sequence.
        /// </summary>
        /// <param name="resolutionScale">The scale to render the sequence at (1 = the sequence is not being scaled)</param>
        /// <param name="yRange">The amount of lines to render</param>
        /// <param name="plot">The plot to refresh after rendering has finished</param>
        /// <param name="scaled">Whether the rendering takes place because of a scaling operation of the view.</param>
        public void Render(double resolutionScale, long yRange, AvaPlot plot, bool scaled)
        {
            // TODO: find a true way to cancel and restart the operation
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();

            // repositioning the part
            Scale = resolutionScale;
            var width = sequence.Shape.Width;
            var height = yRange;
            YEnd = YStart + yRange;

            // rendering in background
            Task.Run(() =>
            {
                try
                {
                    _cancellationToken.Token.ThrowIfCancellationRequested();
                    // drawing a faint overlay if the view is being scaled and therefore rerendered
                    if (scaled && _bitmap is not null)
                    {
                        var tmp = CreateTempBitmap(1, 1, SKColors.Gray.WithAlpha(50));

                        var infoBmp = new SKBitmap(new SKImageInfo(_bitmap.Width, _bitmap.Height));
                        using var canvas = new SKCanvas(infoBmp);
                        canvas.DrawBitmap(_bitmap, new SKPoint(0, 0), Paint);
                        canvas.DrawBitmap(tmp, new SKRectI(0, 0, _bitmap.Width, _bitmap.Height), Paint);

                        _cancellationToken.Token.ThrowIfCancellationRequested();
                        Bitmap = infoBmp;
                    }
                    else
                    {
                        _bitmap = CreateTempBitmap(1, 1, SKColors.Gray);
                        Bitmap = _bitmap;
                    }

                    // rerendering the sequence part
                    // TODO: make independent of seqImg
                    var bmp = seqImg.GetImage(0, sequence.Shape.Width,
                        YStart, YStart + yRange,
                        (int)(width * resolutionScale), (int)(height * resolutionScale),
                        _cancellationToken);

                    Bitmap = bmp;
                    _bitmap = bmp;
                    plot.Refresh();
                }
                catch (OperationCanceledException)
                {
                }
            }, _cancellationToken.Token);
        }

        public void Dispose()
        {
            // cleaning up
            _cancellationToken?.Dispose();
            _bitmap?.Dispose();
            Bitmap?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    private List<SequencePart> _sectionPreviews = [];

    /// <summary>
    /// Gets a rendered sequence part.
    /// </summary>
    /// <param name="index">The index of the part</param>
    /// <returns>A rendered sequence part</returns>
    public SequencePart GetRenderedPart(int index) => _sectionPreviews[index];

    /// <summary>
    /// Gets the amount of sequence parts rendered.
    /// </summary>
    /// <returns></returns>
    public int GetRenderedPartsCount() => _sectionPreviews.Count;

    private (long min, long max) _GetPreviewYRange()
    {
        return (_sectionPreviews[0].YStart, _sectionPreviews[^1].YEnd);
    }

    private readonly Sequence _sequence;
    private readonly AvaPlot _avaPlot;

    /// <summary>
    /// Creates a new SequenceImage and starts rendering at position 0.
    /// </summary>
    /// <param name="sequence">The sequence used</param>
    /// <param name="avaPlot">The plot to refresh on when background rendering is finished</param>
    public SequenceImage(Sequence sequence, AvaPlot avaPlot)
    {
        _avaPlot = avaPlot;
        _sequence = sequence;
        MinPreloadedSections = Math.Min(MinPreloadedSections, (int)(_sequence.Shape.Height / SectionHeight));
        for (var i = 0; i < MinPreloadedSections; i++)
        {
            _sectionPreviews.Add(new SequencePart(_sequence, this, i * SectionHeight));
            _sectionPreviews[i].Render(0.25, SectionHeight, avaPlot, false);
        }
    }

    /// <summary>
    /// Renders the sequence parts dynamically based on the currently viewable area.
    /// Forces the plot to refresh when rendering is finished.
    /// </summary>
    /// <param name="minY">The visible start line</param>
    /// <param name="maxY">The visible end line</param>
    /// <param name="canvasHeight">The actual height of the canvas</param>
    public void Update(long minY, long maxY, int canvasHeight)
    {
        // Computes the current scaling in 4 different steps (0.25, 0.5, 0.75, 1)
        var scale = Math.Min((double)canvasHeight / (maxY - minY), 1);
        scale *= 4;
        scale = Math.Floor(scale);
        scale /= 4;
        scale = Math.Max(0.25, scale);

        // determines the position (in percent) of the current visible area vs the already rendered view
        // used to decide whether to preload parts
        var renderedRange = _GetPreviewYRange();
        var visibleRangeMid = (maxY - minY) / 2 + minY;
        var positionInRenderedRange =
            (double)(visibleRangeMid - renderedRange.min) / (renderedRange.max - renderedRange.min);

        // preload to higher visible lines
        if ((positionInRenderedRange > 0.66 || renderedRange.max < maxY) && renderedRange.max < _sequence.Shape.Height)
        {
            // removes invisible images if necessary
            while (GetRenderedPartsCount() > MinPreloadedSections && _sectionPreviews[0].YEnd < minY)
            {
                _sectionPreviews[0].Dispose();
                _sectionPreviews.RemoveAt(0);
            }

            // add image
            var preview = new SequencePart(_sequence, this, renderedRange.max);
            _sectionPreviews.Add(preview);
            var range = Math.Min(SectionHeight, _sequence.Shape.Height - renderedRange.max);
            preview.Render(0.25, range, _avaPlot, false);
        }

        // basically the same as previous, but for the other side of the view
        // preload to lower visible lines
        if ((positionInRenderedRange < 0.33 || renderedRange.min > minY) && renderedRange.min > 0)
        {
            // removes invisible images if necessary
            while (GetRenderedPartsCount() > MinPreloadedSections && _sectionPreviews[^1].YStart > maxY)
            {
                _sectionPreviews[^1].Dispose();
                _sectionPreviews.RemoveAt(_sectionPreviews.Count - 1);
            }

            // add image
            var start = Math.Max(renderedRange.min - SectionHeight, 0);
            var preview = new SequencePart(_sequence, this, start);
            _sectionPreviews.Insert(0, preview);

            var range = Math.Min(SectionHeight, renderedRange.min);
            preview.Render(0.25, range, _avaPlot, false);
        }

        // account for zooming
        foreach (var preview in _sectionPreviews.Where((preview) =>
                     (preview.YStart >= minY && preview.YStart <= maxY ||
                      preview.YEnd >= minY && preview.YEnd <= maxY ||
                      preview.YStart <= minY && preview.YEnd >= maxY) && preview.Scale < scale))
        {
            // rerender part if scale is not up to date
            preview.Render(scale, preview.YEnd - preview.YStart, _avaPlot, true);
        }
    }

    public void Dispose()
    {
        // doing cleanup
        for (var i = _sectionPreviews.Count - 1; i >= 0; i--)
        {
            _sectionPreviews[i].Dispose();
            _sectionPreviews.RemoveAt(i);
        }

        _sequence.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Renders a specific part of the sequence into a bitmap with desired width and height.
    /// </summary>
    /// <param name="startX">The leftmost position</param>
    /// <param name="endX">The rightmost position</param>
    /// <param name="startLine">The topmost position</param>
    /// <param name="endLine">The bottommost position</param>
    /// <param name="width">The width of the resulting image</param>
    /// <param name="height">The height of the resulting image</param>
    /// <param name="tokenSource">A cancellation-token to cancel the execution</param>
    /// <returns>The part of the sequence rendered to a bitmap</returns>
    private SKBitmap GetImage(long startX, long endX, long startLine, long endLine, int width,
        int height,
        CancellationTokenSource? tokenSource = null)
    {
        // TODO: change
        SequenceRenderer renderer = new ChannelMapRenderer(0, 255, 2, 1, 0);
        /*
        renderer = new HeatMapRendererRB(0, 1, 1);
        */

        // clamping all values
        startX = Math.Clamp(startX, 0, _sequence.Shape.Width);
        endX = Math.Clamp(endX, 0, _sequence.Shape.Width);
        startLine = Math.Clamp(startLine, 0, _sequence.Shape.Height);
        endLine = Math.Clamp(endLine, 0, _sequence.Shape.Height);

        width = (int)Math.Clamp(width, 0, endX - startX);
        height = (int)Math.Clamp(height, 0, endLine - startLine);

        BgraBitmap bitmap = new(width, height);

        // calculating all x positions actually processed
        var xs = new long[width];
        for (var i = 0; i < width; i++)
        {
            xs[i] = startX + i * (endX - startX) / width;
        }

        // using parallelism to render
        Parallel.For(0, height, j =>
        {
            try
            {
                // calculating the actual line currently processed
                var line = startLine + j * (endLine - startLine) / height;

                // rendering each pixel using a renderer
                var data = renderer.RenderPixels(_sequence, xs, line, tokenSource);

                var span = bitmap.GetPixelSpan();
                var pixels = MemoryMarshal.Cast<byte, BGRA>(span);

                // putting the data inside the bitmap
                for (var i = 0; i < width; i++)
                {
                    tokenSource?.Token.ThrowIfCancellationRequested();
                    pixels[j * width + i] = new BGRA()
                    {
                        R = data[i, 1],
                        G = data[i, 2],
                        B = data[i, 3],
                        A = data[i, 0]
                    };
                }
            }
            catch (OperationCanceledException)
            {
            }
        });


        return bitmap;
    }

    private static SKBitmap CreateTempBitmap(int width, int height, SKColor color)
    {
        var bitmap = new SKBitmap(new SKImageInfo(width, height));
        var canvas = new SKCanvas(bitmap);
        canvas.Clear(color);
        return bitmap;
    }
}