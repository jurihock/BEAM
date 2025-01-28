// (c) Paul Stier, 2025

using System;
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
    public class ImagePreview(
        SKBitmap bitmap,
        long renderWidth,
        long renderHeight,
        long yPos,
        float scale,
        SequenceImage seqImg)
    {
        public SKBitmap Bitmap { get; private set; } = bitmap;
        public long RenderWidth { get; private set; } = renderWidth;
        public long RenderHeight { get; private set; } = renderHeight;
        public long YPos { get; set; } = yPos;
        public float Scale { get; } = scale;

        private CancellationTokenSource? _cancellationToken;

        public void Render(Sequence sequence, int height, long visibleYRange, AvaPlot plot)
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();
            Task.Run(() =>
            {
                if (_cancellationToken.Token.IsCancellationRequested) return;
                Bitmap = SequenceImage.CreateTempBitmap(1, 1, SKColors.Gray);

                RenderHeight = height;
                var bmp = seqImg.GetImage(0, sequence.Shape.Width, YPos, YPos + height, height, height,
                    _cancellationToken);
                if (_cancellationToken.Token.IsCancellationRequested) return;
                Bitmap = bmp;
                plot.Refresh();
                Console.WriteLine($"{height}");
            }, _cancellationToken.Token);
        }
    }

    private ImagePreview[] _imagePreviews = new ImagePreview[11];
    public ImagePreview GetPreview(int index) => _imagePreviews[index];
    public int GetPreviewCount() => _imagePreviews.Length;

    private int _GetMaxImageIndex()
    {
        var max = _imagePreviews[0].YPos;
        var idx = 0;

        for (var i = 1; i < _imagePreviews.Length; i++)
        {
            var pos = _imagePreviews[i].YPos;
            if (pos <= max) continue;

            max = pos;
            idx = i;
        }

        return idx;
    }

    private int _GetMinImageIndex()
    {
        var min = _imagePreviews[0].YPos;
        var idx = 0;

        for (var i = 1; i < _imagePreviews.Length; i++)
        {
            var pos = _imagePreviews[i].YPos;
            if (pos >= min) continue;

            min = pos;
            idx = i;
        }

        return idx;
    }

    private (long min, long max) _GetPreviewYRange()
    {
        var min = _imagePreviews[0].YPos;
        var max = _imagePreviews[0].YPos + _imagePreviews[0].RenderHeight;

        for (var i = 1; i < _imagePreviews.Length; i++)
        {
            var pos = _imagePreviews[i].YPos;
            if (pos < min) min = pos;

            var maxPos = pos + _imagePreviews[i].RenderHeight;
            if (maxPos > max) max = maxPos;
        }

        return (min, max);
    }

    private long lastMinY;
    private long lastMaxY;

    private readonly Sequence _sequence;
    private readonly AvaPlot _avaPlot;

    public SequenceImage(Sequence sequence, AvaPlot avaPlot)
    {
        _avaPlot = avaPlot;
        _sequence = sequence;
        for (var i = 0; i < _imagePreviews.Length; i++)
        {
            _imagePreviews[i] = new ImagePreview(CreateTempBitmap(500, 500, SKColors.Brown), sequence.Shape.Width, 500,
                i * 500,
                1, this);
        }
    }

    public void Update(long minY, long maxY, int canvasWidth, int canvasHeight)
    {
        var range = _GetPreviewYRange();

        minY = Math.Clamp(minY, 0, _sequence.Shape.Height);
        maxY = Math.Clamp(maxY, 0, _sequence.Shape.Height - minY);
        canvasHeight = Math.Max(canvasHeight, canvasWidth);

        if (minY > (range.max - range.min) / 3 * 2 + range.min)
        {
            var img = _imagePreviews[_GetMinImageIndex()];
            img.YPos = range.max;
            img.Render(_sequence, canvasHeight, maxY - minY, _avaPlot);
        }
        else if (minY < range.min)
        {
            var img = _imagePreviews[_GetMaxImageIndex()];
            img.YPos = range.min - canvasHeight;
            img.Render(_sequence, canvasHeight, maxY - minY, _avaPlot);
        }

        lastMinY = minY;
        lastMaxY = maxY;
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

            //var data = renderer.RenderPixels(_sequence, xs, line, tokenSource);
            var data = renderer.RenderPixels(_sequence, xs, line);

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