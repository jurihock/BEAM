using BEAM.Datatypes.Color;

namespace BEAM.Controls;


/// <summary>
/// Represents a Channel for the ArgMaxRenderer, containing information for the display of the channel:
/// the index of the Channel
/// the HSV value of the color for the channel
/// if the channel is used for the ArgMaxRenderer (e.g.: alpha channel should be excluded)
/// </summary>
public class Channel
{
    private static readonly HueColorLut hcl = new HueColorLut();
    
    public Channel(bool isUsed, double hsvValue)
    {
        HSVValue = hsvValue;
        IsUsedForArgMax = isUsed;
    }

    public Channel(int index)
    {
        HSVValue = hcl[index].ToHsv().V;
    }

    public Channel()
    {
        HSVValue = 0;
    }

    public double HSVValue { get; set; } = 0;

    public bool IsUsedForArgMax { get; set; } = true;

    public Channel Clone()
    {
        return new Channel(IsUsedForArgMax, HSVValue);
    }
}