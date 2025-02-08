using System;
using System.Collections.Generic;
using BEAM.Image;

namespace BEAM.ImageSequence;

public class CutSequence(List<string> imagePaths, string name, long offset, DiskSequence originalSequence) : DiskSequence(imagePaths, name)
{
    
    private ImageShape? _shape;
    
    public new double GetPixel(long x, long y, int channel)
    {
        return originalSequence.GetPixel(x, y + offset, channel);
    }
    
    public new double[] GetPixel(long x, long y)
    {
        return originalSequence.GetPixel(x, y + offset);
    }
    
    public new double[] GetPixel(long x, long y, int[] channels)
    {
        return originalSequence.GetPixel(x, y + offset, channels);
    }
    
    public new LineImage GetPixelLineData(long line, int[] channels)
    {
        return originalSequence.GetPixelLineData(line + offset, channels);
    }
    
    public LineImage GetPixelLineData(long[] xs, long line, int[] channels)
    {
        return originalSequence.GetPixelLineData(xs, line + offset, channels);
    }

    public new string GetName()
    {
        return name;
    }

    protected internal override IImage LoadImage(int index)
    {
        var singleImageHeight = originalSequence.SingleImageHeight;
        return originalSequence.LoadImage((int)( index + offset / singleImageHeight));
    }

    protected internal override bool InitializeSequence()
    {
        return originalSequence.InitializeSequence();
    }

    public new IImage GetImage(int index)
    {
        if (index < 0 || index >= originalSequence.getLoadedImageCount() - offset / originalSequence.SingleImageHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range [0, amount of images)");
        }

        return originalSequence.GetImage((int)( index + offset / originalSequence.SingleImageHeight));
    }

    public ImageShape Shape
    {
        get
        {
            if (_shape is not null)
            {
                return _shape.Value;
            }

            _InitializeShape();
            return _shape.Value;
        }
    }
    
    private void _InitializeShape()
    {
        var originalShape = originalSequence.Shape;
        _shape = new ImageShape(originalShape.Width, originalShape.Height - offset, originalShape.Channels);
    }
    
    
}