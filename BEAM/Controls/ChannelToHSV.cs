using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using BEAM.Datatypes.Color;
using BEAM.Exceptions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

/// <summary>
/// Represents a Channel for the ArgMaxRenderer, containing information for the display of the channel:
/// the HSV value of the color for the channel
/// if the channel is used for the ArgMaxRenderer (e.g.: alpha channel should be excluded)
/// </summary>
public class ChannelToHSV : ObservableObject
{
    private static readonly HueColorLut hcl = new HueColorLut();
    private double _hsvValue;
    private bool _isUsedForArgMax = true;


    public double HSVValue
    {
        get => _hsvValue;
        set => SetProperty(ref _hsvValue, value);
    }

    public bool IsUsedForArgMax
    {
        get => _isUsedForArgMax;
        set => SetProperty(ref _isUsedForArgMax, value);
    }

    private Color? _color;

    public Color AvaColor
    {
        get
        {
            // calculate the default color
            if (_color == null)
            {
                var bgr = hcl[HSVValue];
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