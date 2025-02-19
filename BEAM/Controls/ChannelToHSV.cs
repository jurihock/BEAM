using BEAM.Datatypes.Color;
using BEAM.Exceptions;

namespace BEAM.Controls;


/// <summary>
/// Represents a Channel for the ArgMaxRenderer, containing information for the display of the channel:
/// the HSV value of the color for the channel
/// if the channel is used for the ArgMaxRenderer (e.g.: alpha channel should be excluded)
/// </summary>
public class ChannelToHSV
{
    private static readonly HueColorLut hcl = new HueColorLut();
    
    public ChannelToHSV(bool isUsed, double hsvValue)
    {
        HSVValue = hsvValue;
        IsUsedForArgMax = isUsed;
    }

    public ChannelToHSV(int index)
    {
        if (index < 0)
        {
            throw new ChannelException("Negative Channel index given!");
        }
        
        HSVValue = hcl[index].ToHsv().V;
        Index = index;
    }
    
    public int Index { get; set; }
    
    public double HSVValue { get; set; } = 0;

    public bool IsUsedForArgMax { get; set; } = true;

    public ChannelToHSV Clone()
    {
        return new ChannelToHSV(IsUsedForArgMax, HSVValue);
    }
}