using Avalonia.Media;
using BEAM.Datatypes.Color;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

/// <summary>
/// Represents a Channel for the ArgMaxRenderer, containing information for the display of the channel:
/// the HSV value of the color for the channel
/// if the channel is used for the ArgMaxRenderer (e.g.: alpha channel should be excluded)
/// </summary>
public class ChannelToHSV : ObservableObject
{
    private static readonly HueColorLut Hcl = new HueColorLut();


    public double HSVValue
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsUsedForArgMax
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    private Color? _color;

    public Color AvaColor
    {
        get
        {
            // calculate the default color
            if (_color == null)
            {
                var bgr = Hcl[HSVValue];
                _color = Color.FromRgb(bgr.R, bgr.G, bgr.B);
            }

            return _color.Value;
        }
        set { _color = value; }
    }

    public ChannelToHSV(double hsvValue, bool isUsed = true)
    {
        HSVValue = hsvValue;
        IsUsedForArgMax = isUsed;
    }

    public ChannelToHSV Clone()
    {
        return new ChannelToHSV(HSVValue, IsUsedForArgMax);
    }
}