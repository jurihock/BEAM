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
public class ChannelToHSV : INotifyPropertyChanged
{
    private static readonly HueColorLut hcl = new HueColorLut();
        
    public int Index { get; set; }
    
    public double HSVValue { get; set; } = 0;

    public bool IsUsedForArgMax { get; set; } = true;

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

    public ChannelToHSV Clone()
    {
        return new ChannelToHSV(IsUsedForArgMax, HSVValue);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}