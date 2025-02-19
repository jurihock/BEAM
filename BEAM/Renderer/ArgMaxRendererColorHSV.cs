using System.Linq;
using System.Threading;
using BEAM.Controls;
using BEAM.Datatypes;
using BEAM.Datatypes.Color;
using BEAM.Exceptions;
using BEAM.Image;
using BEAM.ImageSequence;

namespace BEAM.Renderer;

/// <summary>
/// An ArgMaxRenderer, which maps the channel number to colors, chosen by the user.
/// </summary>
/// <param name="minimumOfIntensityRange"></param>
/// <param name="maximumOfIntensityRange"></param>
public class ArgMaxRendererColorHSV(double minimumOfIntensityRange, double maximumOfIntensityRange) : ArgMaxRenderer(minimumOfIntensityRange, maximumOfIntensityRange)
{
    private ChannelHSVMap _channelHsvMap;
        
    public override RenderTypes GetRenderType()
    {
        return RenderTypes.ArgMaxRendererColorHsva;
    }

    protected override SequenceRenderer Create(int minIntensityRange, int maximumOfIntensityRange, double[] displayParameters)
    {
        return new ArgMaxRendererColorHSV(minIntensityRange, maximumOfIntensityRange);
    }

    protected override bool CheckParameters(double[] displayParameters, IImage image)
    {
        return displayParameters.Length == 0;
    }

    public override object Clone()
    {
        return new ArgMaxRendererColorHSV(minimumOfIntensityRange, maximumOfIntensityRange);
    }

    /// <summary>
    /// Converts the channel position of the channel with the highest intensity into an BGR value.
    /// </summary>
    /// <param name="channelNumber"></param>
    /// <param name="amountChannels"></param>
    /// <returns></returns>
    protected override BGR GetColor(int channelNumber, int amountChannels)
    {
        //calculate the relative position of the given channel in all channels
        // and map it to an int intensity between 0 and 255 for the RGB values.
        var intensity = ((double)(channelNumber + 1) / (double)amountChannels); // in [0, 1]
        var color = new HueColorLut();
        return color[intensity];
    }
    
    public override BGR RenderPixel(ISequence sequence, long x, long y)
    {
        var channels = sequence.GetPixel(x, y);
        if (channels.Length != _channelHsvMap.AmountChannels)
        {
            throw new ChannelException("Channel count mismatch for sequence and ChannelHSVMap of ArgMax!");
        }
        
        var argMaxChannel = channels
            .Select((value, index) => new { Value = value, Index = index }) // Create an anonymous type with value and index
            .Where(x => _channelHsvMap.isChannelUsed(x.Index)) // Filter based on ChannelUsed
            .Select(x => x.Value) // Select only the values
            .ToArray() // Convert to array
            .ArgMax(); // Call ArgMax on the filtered array
        
        var color = GetColor(argMaxChannel, channels.Length);
        
        return color;
    }

    public override BGR[] RenderPixels(ISequence sequence, long[] xs, long y,
        CancellationTokenSource? tokenSource = null)
    {
        var channels = new int[sequence.Shape.Channels];
        for (var i = 0; i < sequence.Shape.Channels; i++)
        {
            if (_channelHsvMap.isChannelUsed(i)) // ignore the alpa channel
            {
                channels[i] = i;
            }
        }

        var line = sequence.GetPixelLineData(xs, y, channels);
        var data = new BGR[xs.Length];
        
        for (var x = 0; x < xs.Length; x++)
        {
            tokenSource?.Token.ThrowIfCancellationRequested();

            var argMaxChannel = line.GetPixel(x, 0)
                .Select((value, index) => new { Value = value, Index = index }) // Create an anonymous type with value and index
                .Where(x => _channelHsvMap.isChannelUsed(x.Index)) // Filter based on ChannelUsed
                .Select(x => x.Value) // Select only the values
                .ToArray() // Convert to array
                .ArgMax(); // Call ArgMax on the filtered array
            
            var color = _channelHsvMap.GetColorBGR(argMaxChannel);

            data[x] = color;
        }

        return data;
    }

    public override string GetName()
    {
        return $"{base.GetName()} (Color HSV)";
    }
}