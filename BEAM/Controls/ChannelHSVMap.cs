using System.Linq;
using BEAM.Datatypes.Color;

namespace BEAM.Controls;

/// <summary>
/// Stores the values of colors in the HSV spectrum in an array.
/// Value i represents the value for channel i (0 -> first channel)
/// </summary>
public class ChannelHSVMap
{
    /// <summary>
    /// Stores the information (index, color, is displayed) of each channel
    ///  
    /// </summary>
    private Channel[] channels;
    
    private HueColorLut hcl = new HueColorLut();
    public int AmountChannels { get; init; }

    public ChannelHSVMap(int maxAmountChannels)
    {
        channels = new Channel[maxAmountChannels];
        for (var i = 0; i < maxAmountChannels; i++)
        {
            channels[i] = new Channel(i);
        }
    }

    /// <summary>
    /// Returns color for the channel as a HSV color.
    /// </summary>
    /// <param name="channel">The index of channel (0 ist first channel)</param>
    /// <returns>The color associated with this channel as a HSV color.</returns>
    public HSV GetColorHSV(int channel)
    {
        return hcl[channels[channel].HSVValue].ToHsv();
    }
    
    /// <summary>
    /// Returns color for the channel as a BGR color.
    /// </summary>
    /// <param name="channel">The index of channel (0 ist first channel)</param>
    /// <returns>The color associated with this channel as a BGR color.</returns>
    public BGR GetColorBGR(int channel)
    {
        return hcl[channels[channel].HSVValue];
    }

    public void SetColor(int channel, HSV color)
    {
        channels[channel].HSVValue = color.V;
    }

    public void SetColor(int channel, BGR color)
    {
        channels[channel].HSVValue = color.ToHsv().V;
    }

    public bool isChannelUsed(int channel)
    {
        return channels[channel].IsUsedForArgMax;
    }

    public void setUsedChannels(int channel, bool value)
    {
        channels[channel].IsUsedForArgMax = value;
    }

    public int getAmountUsedChannels()
    {
        return channels.Count(value => value.IsUsedForArgMax);
    }

    /// <summary>
    /// Sets the channel
    /// </summary>
    /// <param name="index"></param>
    /// <param name="channel"></param>
    public void setChannel(int index, Channel channel)
    {
        channels[index] = channel;
    }

    /// <summary>
    /// Creates a Clone of the ChannelHSVMap and fills it with the same values.
    /// </summary>
    /// <returns></returns>
    public ChannelHSVMap Clone()
    {
        var clone = new ChannelHSVMap(AmountChannels);
        for (var i = 0; i < AmountChannels; i++)
        {
            clone.setChannel(i, channels[i].Clone());
        }

        return clone;
    }
    
}