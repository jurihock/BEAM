using System.Collections.Generic;
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
    private readonly ChannelToHSV[] _channels;

    private static readonly HueColorLut Hcl = new HueColorLut();
    public int AmountChannels => _channels.Length;

    public ChannelHSVMap(int maxAmountChannels)
    {
        _channels = new ChannelToHSV[maxAmountChannels];
        for (var i = 0; i < maxAmountChannels; i++)
        {
            _channels[i] = new ChannelToHSV(i / (double)maxAmountChannels);
        }
    }

    public ChannelHSVMap(ChannelToHSV[] channels)
    {
        this._channels = channels;
    }

    /// <summary>
    /// Returns color for the channel as a HSV color.
    /// </summary>
    /// <param name="channel">The index of channel (0 ist first channel)</param>
    /// <returns>The color associated with this channel as a HSV color.</returns>
    public HSV GetColorHSV(int channel)
    {
        return Hcl[_channels[channel].HSVValue].ToHsv();
    }

    /// <summary>
    /// Returns color for the channel as a BGR color.
    /// </summary>
    /// <param name="channel">The index of channel (0 ist first channel)</param>
    /// <returns>The color associated with this channel as a BGR color.</returns>
    public BGR GetColorBGR(int channel)
    {
        return Hcl[_channels[channel].HSVValue];
    }

    public void SetColor(int channel, HSV color)
    {
        _channels[channel].HSVValue = color.V;
    }

    public void SetColor(int channel, BGR color)
    {
        _channels[channel].HSVValue = color.ToHsv().V;
    }

    public bool IsChannelUsed(int channel)
    {
        return _channels[channel].IsUsedForArgMax;
    }

    public void SetUsedChannels(int channel, bool value)
    {
        _channels[channel].IsUsedForArgMax = value;
    }

    public int GetAmountUsedChannels()
    {
        return _channels.Count(value => value.IsUsedForArgMax);
    }

    /// <summary>
    /// Returns the indices of the channels used for the ArgMaxRenderer in ascending order.
    /// </summary>
    /// <returns></returns>
    public int[] GetUsedChannels()
    {
        var usedChannels = new List<int>();
        for (int i = 0; i < AmountChannels; i++)
        {
            if (_channels[i].IsUsedForArgMax)
            {
                usedChannels.Add(i);
            }
        }

        return usedChannels.ToArray();
    }

    /// <summary>
    /// Sets the channel to hsv mapping
    /// </summary>
    /// <param name="index"></param>
    /// <param name="channelToHsv"></param>
    public void SetChannel(int index, ChannelToHSV channelToHsv)
    {
        _channels[index] = channelToHsv;
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
            clone.SetChannel(i, _channels[i].Clone());
        }

        return clone;
    }

    /// <summary>
    /// Returns a pointer to the map of the ChannelToHSV array
    /// </summary>
    /// <returns></returns>
    public ChannelToHSV[] ToArray()
    {
        return _channels;
    }

}