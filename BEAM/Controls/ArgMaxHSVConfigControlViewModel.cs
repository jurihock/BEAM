using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using BEAM.Datatypes.Color;
using BEAM.Exceptions;
using BEAM.Models.Log;
using BEAM.Renderer;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

public partial class ArgMaxHSVConfigControlViewModel : ViewModelBase, ISaveControl
{
    private readonly ArgMaxRendererColorHSV _renderer;
    
    public decimal Min { get; set; } = 0;
    
    private ObservableCollection<ChannelToHSV> _obsChannels;

    public ObservableCollection<ChannelToHSV> ObsChannels
    {
        get => _obsChannels;
        set => SetProperty(ref _obsChannels, value);
    }
    
    private int _selectedChannelIndex;

    public int SelectedChannelIndex
    {
        get => _selectedChannelIndex;
        set
        {
            _selectedChannelIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedChannel));
            OnPropertyChanged(nameof(ColorBrush));
        }
    }
    
    public ChannelToHSV SelectedChannel =>ObsChannels[SelectedChannelIndex];

    /// <summary>
    /// Displays the color associated with a channel
    /// </summary>
    public IBrush ColorBrush => new SolidColorBrush(SelectedChannel.AvaColor);
    
    public ArgMaxHSVConfigControlViewModel(ArgMaxRendererColorHSV renderer, SequenceViewModel model)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (model == null) throw new ArgumentNullException(nameof(model));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        
        if (_renderer.getChannelHsvMap().AmountChannels == 0)
        {
            ObsChannels = new ObservableCollection<ChannelToHSV>(new ChannelHSVMap(model.Sequence.Shape.Channels).ToArray());
        }
        else
        {
            ObsChannels = new ObservableCollection<ChannelToHSV>(renderer.getChannelHsvMap().ToArray());
        }
    }
    
    public void Save()
    {
        var map = new ChannelHSVMap(ObsChannels.ToArray());
        if (map.getAmountUsedChannels() == 0)
        {
            // throw exception for logging
            //throw new ChannelException("Channel amount used for ArgMax is zero!");
            return;
        }
        _renderer.UpdateChannelHSVMap(ObsChannels.ToArray());
    }
}