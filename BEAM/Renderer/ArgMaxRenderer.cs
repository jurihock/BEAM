using System.Linq;
using System.Threading;
using BEAM.Datatypes.Color;
using BEAM.Exceptions;
using BEAM.ImageSequence;

namespace BEAM.Renderer;

/// <summary>
/// A Renderer using the ArgMax function. Calculates the channel with the highest intensity and encodes it's number as a
/// color.
/// </summary>
/// <param name="minimumOfIntensityRange"></param>
/// <param name="maximumOfIntensityRange"></param>
public abstract class ArgMaxRenderer(double minimumOfIntensityRange, double maximumOfIntensityRange)
    : SequenceRenderer(minimumOfIntensityRange, maximumOfIntensityRange)
{
    public override BGRA RenderPixel(ISequence sequence, long x, long y)
    {
        var channels = sequence.GetPixel(x, y);
        var argMaxChannel = ArgMax(channels);
        var color = GetColor(argMaxChannel, channels.Length);
        
        return color;
    }

    public override BGRA[] RenderPixels(ISequence sequence, long[] xs, long y, CancellationTokenSource? tokenSource = null)
    {
        var data = new BGRA[xs.Length];
        // TODO: Update to sequence.GetChannelAmount
        var amountChannels = sequence.GetPixel(0, 0).Length;
        var channels = Enumerable.Range(0, amountChannels).ToArray();
        
        var img = sequence.GetPixelLineData(xs, y, channels);

        for (var i = 0; i < xs.Length; i++)
        {
            tokenSource?.Token.ThrowIfCancellationRequested();
            var channel = ArgMax(img.GetPixel(i, 0));
            var color = GetColor(channel, amountChannels);
            data[i] = color;
        }

        return data;
    }

    /// <summary>
    /// Given an Array of channel intensities, return the first index with the highest intensity
    /// </summary>
    /// <param name="channels"></param>
    /// <returns></returns>
    /// <exception cref="ChannelException"></exception>
    private int ArgMax(double[] channels)
    {
        if (channels.Length <= 0)
        {
            throw new ChannelException("Channels must be greater than 0.");
        }
        
        double maxIntensity = MinimumOfIntensityRange;
        int maxPosition = 0;
        for (int i = 0; i < channels.Length; i++)
        {
            if (channels[i] > maxIntensity)
            {
                maxIntensity = channels[i];
                maxPosition = i;
            }
        }

        return maxPosition;
    }
    
    protected abstract BGRA GetColor(int channelNumber, int amountChannels);

    public override string GetName() => "ArgMax";
}