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
    /// Stores the value of the HSV color of the channel i at position i. 
    /// </summary>
    private double[] valueArray;
    
    /// <summary>
    /// Stores at index i, if channel i should be considered for the ArgMax function.
    /// </summary>
    private bool[] usedChannels;
    
    private HueColorLut hcl = new HueColorLut();
    public int AmountChannels { get; init; }

    public ChannelHSVMap(int maxAmountChannels)
    {
        valueArray = new double[maxAmountChannels];
        usedChannels = new bool[maxAmountChannels];
        for (var i = 0; i < valueArray.Length; i++)
        {
            valueArray[i] = i;
            usedChannels[i] = true;
        }
    }

    /// <summary>
    /// Returns color for the channel as a HSV color.
    /// </summary>
    /// <param name="channel">The index of channel (0 ist first channel)</param>
    /// <returns>The color associated with this channel as a HSV color.</returns>
    public HSV GetColorHSV(int channel)
    {
        return hcl[valueArray[channel]].ToHsv();
    }
    
    /// <summary>
    /// Returns color for the channel as a BGR color.
    /// </summary>
    /// <param name="channel">The index of channel (0 ist first channel)</param>
    /// <returns>The color associated with this channel as a BGR color.</returns>
    public BGR GetColorBGR(int channel)
    {
        return hcl[valueArray[channel]];
    }

    public void SetColor(int channel, HSV color)
    {
        valueArray[channel] = color.V;
    }

    public void SetColor(int channel, BGR color)
    {
        valueArray[channel] = color.ToHsv().V;
    }

    public bool isChannelUsed(int channel)
    {
        return usedChannels[channel];
    }

    public void setUsedChannels(int channel, bool value)
    {
        usedChannels[channel] = value;
    }

    public int getAmountUsedChannels()
    {
        return usedChannels.Count(value => value);
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
            clone.setUsedChannels(i, usedChannels[i]);
            clone.SetColor(i, this.GetColorHSV(i));
        }
    }
}