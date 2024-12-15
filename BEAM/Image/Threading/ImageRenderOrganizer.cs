using System;
using System.Threading;
using System.Threading.Tasks;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.Renderer;

namespace BEAM.Image.Threading;

public class ImageRenderOrganizer
{
    private readonly ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true); // Used to pause/resume
    private CancellationTokenSource _cts = new CancellationTokenSource(); // Used to stop
    private readonly object _lockObj = new object();
    
    public RenderedImageExcerpt renderableExcerpt { get; private set; }
    private bool isRenderingPadding = false;
    public RenderedImageExcerpt RenderRGBImage(Sequence? sequence, long imageStartLine, long imageEndLine, double zoomLevel, SequenceRenderer renderer)
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
        if (renderableExcerpt is null || !renderableExcerpt.IsCompatible((width, imageStartLine-  imageEndLine + 1), zoomLevel) || !renderableExcerpt.IsCopyable(imageStartLine))
        {
            long startindex = imageStartLine * width;
            long  endIndex = imageEndLine * width - 1;
            int padding = 100; //TODO: Absolute vs Relative
            var renderedImageExcerpt = new RenderedImageExcerpt(width, imageEndLine - imageStartLine + 1, padding, zoomLevel, (0, imageStartLine)); 

            Parallel.For(startindex, endIndex + 1, new ParallelOptions(){MaxDegreeOfParallelism =Environment.ProcessorCount},(i) =>
            {
                long y = i / width; // Integer division for line
                long x = i % width; // Pixel coord within line
                byte[] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
                renderedImageExcerpt.PixelData[x, y + padding - 1, 0] = pixelRGBValues[0];
                renderedImageExcerpt.PixelData[x, y + padding - 1, 1] = pixelRGBValues[1];
                renderedImageExcerpt.PixelData[x, y + padding - 1, 2] = pixelRGBValues[2];
            });
            this.renderableExcerpt = renderedImageExcerpt;
        } else
        {
            long validCopyRange = imageEndLine - imageStartLine + 1;  //TODO:  We could even try to save the padding and not just the main image data

            long remainingLines = renderableExcerpt.Height;
            if (imageStartLine <= renderableExcerpt.OriginPixel.Item2)
            {
                if (renderableExcerpt.OriginPixel.Item2 - imageStartLine <= renderableExcerpt.Padding)
                {
                    CopyOverlappingArea3D(renderableExcerpt.PixelData, 0, renderableExcerpt.OriginPixel.Item2, 0,
                        renderableExcerpt.Width,renderableExcerpt.Height + renderableExcerpt.Padding, 3, 0, Math.Min(renderableExcerpt.OriginPixel.Item2 - imageStartLine, renderableExcerpt.Padding),0);
                    remainingLines = Math.Max(-renderableExcerpt.Padding + (renderableExcerpt.OriginPixel.Item2 - imageStartLine), 0);
                }
                else
                {
                    
                }
               
            }
            else
            {
                if (imageStartLine - renderableExcerpt.OriginPixel.Item2 <= renderableExcerpt.Padding)
                {
                    
                }
                else
                {
                    
                }
                CopyOverlappingArea3D(renderableExcerpt.PixelData, 0, renderableExcerpt.OriginPixel.Item2, 0,
                    renderableExcerpt.Width,renderableExcerpt.Height + renderableExcerpt.Padding, 3, 0, Math.Min(renderableExcerpt.OriginPixel.Item2 - imageStartLine, renderableExcerpt.Padding),0);
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
                if (i <  renderableExcerpt.Padding * renderableExcerpt.Width)
                {
                    byte[] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
                    renderableExcerpt.PixelData[x, y, 0] = pixelRGBValues[0];
                    renderableExcerpt.PixelData[x, y, 1] = pixelRGBValues[1];
                    renderableExcerpt.PixelData[x, y, 2] = pixelRGBValues[2];
                } else
                {
                    byte[] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Height, 0] = pixelRGBValues[0];
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Height, 1] = pixelRGBValues[1];
                    renderableExcerpt.PixelData[x, y + renderableExcerpt.Height, 2] = pixelRGBValues[2];
                }
            });
        }, _cts.Token);
        
        return renderableExcerpt;

    }

    public Task GeneratePadding(Sequence sequence, SequenceRenderer renderer, RenderedImageExcerpt prevExcerpt)
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
                            byte[] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
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
                                byte[] pixelRGBValues = renderer.RenderPixel(sequence.GetPixel(x, y));
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
    public static void CopyOverlappingArea3D<T>(
        T[,,] array, 
        long sourceX, long sourceY, long sourceZ, 
        long width, long height, long depth, 
        long destX, long destY, long destZ)
    {
        // Validate array bounds and ensure no out-of-bound access
        if (array == null) throw new ArgumentNullException(nameof(array));
        long sizeX = array.GetLength(0);
        long sizeY = array.GetLength(1);
        long sizeZ = array.GetLength(2);

        if (sourceX < 0 || sourceY < 0 || sourceZ < 0 ||
            destX < 0 || destY < 0 || destZ < 0 ||
            sourceX + width > sizeX || sourceY + height > sizeY || sourceZ + depth > sizeZ ||
            destX + width > sizeX || destY + height > sizeY || destZ + depth > sizeZ)
        {
            throw new ArgumentOutOfRangeException("Source or destination region is out of bounds.");
        }

        // Use a temporary buffer to avoid overwriting issues during copying
        T[,,] temp = new T[width, height, depth];

        // Copy the source region to the temporary buffer
        for (long x = 0; x < width; x++)
        {
            for (long y = 0; y < height; y++)
            {
                for (long z = 0; z < depth; z++)
                {
                    temp[x, y, z] = array[sourceX + x, sourceY + y, sourceZ + z];
                }
            }
        }

        // Copy the temporary buffer to the destination region
        for (long x = 0; x < width; x++)
        {
            for (long y = 0; y < height; y++)
            {
                for (long z = 0; z < depth; z++)
                {
                    array[destX + x, destY + y, destZ + z] = temp[x, y, z];
                }
            }
        }
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
        PixelData = new byte[width, 2* padding + height, 3];//TODO: Constant separation
    }

    public bool IsCompatible((long, long) dimension, double zoomLevel, double compThreshold = 1e-7)
    {
        return (dimension.Equals((Width, Height)) && Math.Abs(this.ZoomLevel - zoomLevel) < compThreshold);
    }


    public bool IsCopyable(long newInitialLine)
    {
        return (Math.Abs(OriginPixel.Item2 - newInitialLine) < Height + Padding);
    }
}