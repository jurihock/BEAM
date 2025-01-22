using System;
using BEAM.ImageSequence;

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
    protected SequenceRenderer(int minimumOfIntensityRange, int maximumOfIntensityRange)
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
    protected double NormalizeIntensity(double intensity)
    {
        if (intensity > MaximumOfIntensityRange || intensity < MinimumOfIntensityRange)
        {
            throw new ArgumentException();
        }
        return (intensity - MinimumOfIntensityRange) 
               / (MaximumOfIntensityRange - MinimumOfIntensityRange);
    }

    public abstract byte[] RenderPixel(Sequence sequence, long x, long y);
}