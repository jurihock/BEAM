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
    /// Stores the information (color, is displayed) of each channel
    ///  
    /// </summary>
    private ChannelToHSV[] channels;
    
    private static readonly HueColorLut hcl = new HueColorLut();
    public int AmountChannels { get; init; }

    public ChannelHSVMap(int maxAmountChannels)
    {
        channels = new ChannelToHSV[maxAmountChannels];
        for (var i = 0; i < maxAmountChannels; i++)
        {
            channels[i] = new ChannelToHSV(i);
        }
    }

    public ChannelHSVMap(ChannelToHSV[] channels)
    {
        this.channels = channels;
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
    /// Sets the channel to hsv mapping
    /// </summary>
    /// <param name="index"></param>
    /// <param name="channelToHsv"></param>
    public void setChannel(int index, ChannelToHSV channelToHsv)
    {
        channels[index] = channelToHsv;
    }

    /// <summary>
    /// Returns a clone to the Channel instance at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ChannelToHSV getChannel(int index)
    {
        return channels[index].Clone();
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

    /// <summary>
    /// Returns a pointer to the map of the ChannelToHSV array
    /// </summary>
    /// <returns></returns>
    public ChannelToHSV[] ToArray()
    {
        return channels;
    }
    
}