using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BEAM.Datatypes.Color;
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
        }
    }
    
    public ChannelToHSV SelectedChannel => _obsChannels[SelectedChannelIndex];
    
    public ArgMaxHSVConfigControlViewModel(ArgMaxRendererColorHSV renderer, SequenceViewModel model)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (model == null) throw new ArgumentNullException(nameof(model));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        
        if (_renderer.getChannelHsvMap().AmountChannels == 0)
        {
            _obsChannels = new ObservableCollection<ChannelToHSV>(new ChannelHSVMap(model.Sequence.Shape.Channels).ToArray());
        }
        else
        {
            _obsChannels = new ObservableCollection<ChannelToHSV>(renderer.getChannelHsvMap().ToArray());
        }
    }
    
    public void Save()
    {
        _renderer.UpdateChannelHSVMap(ObsChannels.ToArray());
    }
}