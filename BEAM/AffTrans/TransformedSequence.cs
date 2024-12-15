using BEAM.ImageSequence;

namespace BEAM.AffTrans;

public class TransformedSequence(Sequence originalSequence, Transformer transformer) 
{
    public double [] GetPixel(long x, long y)
    {
        (long transformedX, long transformedY) transformedCoordinate = transformer.Transform(x, y);
        double [] scaledPixel = originalSequence.GetPixel(transformedCoordinate.transformedX, transformedCoordinate.transformedY);
        return scaledPixel;
    }
    
    public double GetPixel(long x, long y, byte channel)
    {
        (long transformedX, long transformedY) transformedCoordinate = transformer.Transform(x, y);
        double scaledPixel = originalSequence.GetPixel(transformedCoordinate.transformedX, transformedCoordinate.transformedY, channel);
        return scaledPixel;
    }

    public double[,] GetPixelLine(long y)
    {
        long transformedY = transformer.TransformY(y);
        double [,] scaledPixel = originalSequence.GetPixelLine(transformedY);
        return scaledPixel;
    }
    
}