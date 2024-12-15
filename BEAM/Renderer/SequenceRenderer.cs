using System;

namespace BEAM.Renderer;

public abstract class SequenceRenderer
{
    protected int MinimumOfIntensityRange { get; init; }
    protected int MaximumOfIntensityRange { get; init; }
    
    /// <summary>
    /// Set the Minimum- and Maximum values for the intensity values (e.g. 0 - 1 or 0 - 255)
    /// This intensity is given by the user
    /// </summary>
    /// <param name="minimumOfIntensityRange"></param>
    /// <param name="maximumOfIntensityRange"></param>
    public SequenceRenderer(int minimumOfIntensityRange, int maximumOfIntensityRange)
    {
        if (maximumOfIntensityRange <= minimumOfIntensityRange)
        {
            throw new ArgumentException("Given maximumOfIntensityRange must be greater than " +
                                        "to minimumOfIntensityRange for color intensities rendering.");
        }
        MinimumOfIntensityRange = minimumOfIntensityRange;
        MaximumOfIntensityRange = maximumOfIntensityRange;
    }

    /// <summary>
    /// Normalizes the intensity of a given intensity to a value between 0 and 1.
    /// Normalization uses the Minimum- and MaximumOfIntensityRange
    /// </summary>
    /// <param name="intensity">The unnormalized intensity of a channel</param>
    /// <returns>The </returns>
    public double NormalizeIntensity(double intensity)
    {
        if (intensity > MaximumOfIntensityRange || intensity < MinimumOfIntensityRange)
        {
            throw new ArgumentException();
        }
        return (intensity - MinimumOfIntensityRange) 
               / (MaximumOfIntensityRange - MinimumOfIntensityRange);
    }
    
    /// <summary>
    /// Returns an array of size 4. These three values
    /// equal the A, R, G and B value of the pixel
    /// A is the transparency value (A = 255 --> not transparent)
    /// </summary>
    /// <param name="channels"></param>
    /// <param name="displayParameters"><\param>
    /// <returns></returns>
    public abstract byte[] RenderPixel(double[] channels, double[] displayParameters);
}