using System;
using System.Threading;
using System.Threading.Tasks;
using BEAM.Exceptions;
using BEAM.ImageSequence;

namespace BEAM.Image.Threading;

public class ImageRenderOrganizer
{
    private readonly ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true); // Used to pause/resume
    private readonly CancellationTokenSource _cts = new CancellationTokenSource(); // Used to stop
    private readonly object _lockObj = new object();
    
    public RenderedImageExcerpt renderableExcerpt { get; private set; }
    private bool isRenderingPadding = false;
    public RenderedImageExcerpt RenderRGBImage(Sequence? sequence, long imageStartLine, long imageEndLine, double zoomLevel, ISequenceRenderer renderer)
    {

        if (sequence is null || sequence.Shape.Width <= 0 || sequence.Shape.Height < 0)
        {
            throw new ImageDimensionException();
        } 
        if (imageEndLine == imageStartLine || imageStartLine > sequence.Shape.Height)
        {
            throw new ArgumentException();
        }

        long width = sequence.Shape.Width;
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        if (renderableExcerpt is null || !renderer.IsCompatible((width, imageStartLine-  imageEndLine + 1), zoomLevel))
        {
            long startindex = imageStartLine * width;
            long  endIndex = imageEndLine * width - 1;
            int padding = 100; //TODO Absolute vs Relative
            var renderedImageExcerpt = new RenderedImageExcerpt(width, imageEndLine - imageStartLine + 1, padding, zoomLevel, (0, imageStartLine)); 

            Parallel.For(startindex, endIndex + 1, new ParallelOptions(){MaxDegreeOfParallelism =Environment.ProcessorCount},(i) =>
            {
                long y = i / width; // Integer division for line
                long x = i % width; // Pixel coord within line
                double[3] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
                renderedImageExcerpt.PixelData[x, y + padding - 1, 0] = pixelRGBValues[0];
                renderedImageExcerpt.PixelData[x, y + padding - 1, 1] = pixelRGBValues[1];
                renderedImageExcerpt.PixelData[x, y + padding - 1, 2] = pixelRGBValues[2];
            });
            this.renderableExcerpt = renderedImageExcerpt;
        } else
        {
            lock (_lockObj)
            {
                
            }
        }
        
        ParallelOptions options = new ParallelOptions
        {
            CancellationToken = _cts.Token,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
        
        Task fillPadding = Task.Run(() =>
        {
            return Parallel.For(Math.Max(0, (renderableExcerpt.Padding- renderableExcerpt.OriginPixel.Item2) * renderableExcerpt.Width),
                (renderableExcerpt.Padding + Math.Min(renderableExcerpt.Padding, sequence.Shape.Height - (renderableExcerpt.OriginPixel.Item2  + renderableExcerpt.Height)))* renderableExcerpt.Width, 
                options, (i) =>
            {
                long y = i / renderableExcerpt.Width;
                long x = i % renderableExcerpt.Width;
                if (y - renderableExcerpt.Padding < 0)
                if (i <  renderableExcerpt.Padding * renderableExcerpt.Width)
                {
                    double[3] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Padding, 0] = pixelRGBValues[0];
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Padding, 1] = pixelRGBValues[1];
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Padding, 2] = pixelRGBValues[2];
                } else
                {
                    double[3] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Padding - 1, 0] = pixelRGBValues[0];
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Padding - 1, 1] = pixelRGBValues[1];
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Padding - 1, 2] = pixelRGBValues[2];
                }
            });
        }, _cts.Token);

        return null;

    }

    public Task GeneratePadding(Sequence sequence, ISequenceRenderer renderer, RenderedImageExcerpt prevExcerpt)
    {
        return Task.Run(() =>
        {
            try
            {
                for (int y = 0; y < prevExcerpt.Height + 2 * prevExcerpt.Padding; y++)
                {
                    // Stop if canceled
                    _cts.Token.ThrowIfCancellationRequested();

                    // Pause if requested
                    _pauseEvent.Wait(_cts.Token);

                    // Fill top padding
                    if (y < prevExcerpt.Padding)
                    {

                        for (int x = 0; x < prevExcerpt.Width; x++)
                        {
                            //TODO Dummy
                            double[3] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
                            prevExcerpt.PixelData[x, y, 0] = pixelRGBValues[0];
                            prevExcerpt.PixelData[x, y, 1] = pixelRGBValues[1];
                            prevExcerpt.PixelData[x, y, 2] = pixelRGBValues[2];
                        }

                    }
                    // Fill bottom padding
                    else if (y >= prevExcerpt.Height + prevExcerpt.Padding)
                    {
                        //lock (lockObj)
                        {
                            for (int x = 0; x < prevExcerpt.Width; x++)
                            {
                                //TODO Dummy
                                double[3] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
                                prevExcerpt.PixelData[x, y, 0] = pixelRGBValues[0];
                                prevExcerpt.PixelData[x, y, 1] = pixelRGBValues[1];
                                prevExcerpt.PixelData[x, y, 2] = pixelRGBValues[2];
                            }
                        }
                    }
                }

                Console.WriteLine("Padding filling complete.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Padding filling stopped.");
            }
        });

    }
    /// <summary>
    /// Pauses padding creation.
    /// </summary>
    public void PausePadding()
    {
        Console.WriteLine("Pausing padding creation...");
        _pauseEvent.Reset();
    }

    /// <summary>
    /// Resumes padding creation.
    /// </summary>
    public void ResumePadding()
    {
        Console.WriteLine("Resuming padding creation...");
        _pauseEvent.Set();
    }

    /// <summary>
    /// Stops padding creation completely.
    /// </summary>
    public void StopPadding()
    {
        Console.WriteLine("Stopping padding creation...");
        _cts.Cancel();
    }
}

public class RenderedImageExcerpt
{
    public byte[,,] PixelData;
    public int Padding {get; private set;}
    public long Width{get; private set;}
    public long Height{get; private set;}
    public double ZoomLevel{get; private set;}
    public (long, long) OriginPixel{get; private set;}
    public RenderedImageExcerpt(long width, long height, int padding, double zoomLevel, (long, long) upperLeftPixel)
    {
        this.Padding = padding;
        this.Width = width;
        this.Height = height;
        this.ZoomLevel = zoomLevel;
        this.OriginPixel = upperLeftPixel;
        PixelData = new byte[width, 2* padding + height, 3];//TODO Constant separation
    }

    public bool IsCompatible((long, long) dimension, double zoomLevel, double compThreshold = 1e-7)
    {
        return (dimension.Equals((Width, Height)) && Math.Abs(this.ZoomLevel - zoomLevel) < compThreshold);
    }

    
    
}