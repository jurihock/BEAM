using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

    public Avalonia.Media.Color AvaColor
    {
        // Calculate the Avalonia Color from the HSVValue
        get
        {
            var bgr = hcl[HSVValue];
            return Avalonia.Media.Color.FromRgb(bgr.R, bgr.G, bgr.B);
        }
        // Update the HSVValue
        set => HSVValue = value.ToHsv().V;
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