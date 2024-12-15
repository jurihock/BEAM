using System;
using System.IO;
using BEAM.Exceptions;

namespace BEAM.Renderer;

/// <summary>
/// General abstract class for a HeatMapRenderer.
/// It implements the function to Render a pixel.
/// The concrete function, that converts the channel intensity to a color is implemented by the subclasses.
/// </summary>
public abstract class HeatMapRenderer : SequenceRenderer
{
    /// <summary>
    /// The parameters are min and max value of the channels intensity.
    /// This determines the range of distinguished intensities by the heatmap
    /// </summary>
    private static readonly int AmountParameters = 2;

    protected HeatMapRenderer(int minimumOfIntensityRange, int maximumOfIntensityRange, int channel) : base(minimumOfIntensityRange, maximumOfIntensityRange)
    {
        Channel = channel;
    }

    /// <summary>
    /// The channel that is used in the HeatMap for the intensity of the HeatMap.
    /// </summary>
    protected int Channel {get; set;}
    
    
    /// <summary>
    /// Given the channels of a pixel, the lower and the upper limit of the HeatMap,
    /// render the RGB encoding of the intensity of this channel in the HeatMap.
    /// </summary>
    /// <param name="channels"></param>
    /// <param name="displayParameters"></param>
    /// <returns>(A, R, G, B) values of the HeatMap. A = 0 fully transparent, A = 255 no opacity</returns>
    /// <exception cref="ImageDimensionException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public override byte[] RenderPixel(double[] channels, double[] displayParameters)
    {
        if (Channel >= channels.Length)
        {
            throw new ChannelException("Invalid channel nr.");
        }

        if (AmountParameters != displayParameters.Length)
        {
            throw new InvalidDataException("Invalid amount parameters.");
        }

        double intensity = channels[Channel];
        
        // calculate the specific for the heatmap for the given intensity
        return GetColor(intensity, displayParameters[0], displayParameters[1]);
    }

    /// <summary>
    /// The specific function that converts the intensity of a channel into a Color.
    /// Is implemented by a subclass.
    /// </summary>
    /// <param name="value">The intensity of the chosen channel</param>
    /// <param name="min">The highest intensity of the channel, that is displayed as the coldest intensity.</param>
    /// <param name="max">The lowest intensity of the channel, that is displayed as the highest intensity.</param>
    /// <returns>The ARGB values of the final Color to be displayed.
    /// (A, R, G, B) each color from 0 - 255. A = 0 : fully transparent</returns>
    protected abstract byte[] GetColor(double value, double min, double max);
}