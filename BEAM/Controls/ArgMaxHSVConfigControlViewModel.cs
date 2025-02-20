using System;
using System.Collections.ObjectModel;
using BEAM.Datatypes.Color;
using BEAM.Renderer;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

public partial class ArgMaxHSVConfigControlViewModel
    : ViewModelBase, ISaveControl
{
    private readonly ArgMaxRendererColorHSV _renderer;

    private ChannelToHSV _selectedItem;
    
    public ChannelToHSV SelectedItem 
    { get => _selectedItem; 
        set => SetProperty(ref _selectedItem, value); }
    
    [ObservableProperty] public partial ObservableCollection<ChannelToHSV> ObsChannels { get; set; } = [];
    
    public ArgMaxHSVConfigControlViewModel(ArgMaxRendererColorHSV renderer, SequenceViewModel model)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (model == null) throw new ArgumentNullException(nameof(model));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));

        ChannelToHSV[] channels;
        
        if (_renderer.getChannelHsvMap().AmountChannels == 0)
        {
            channels = new ChannelHSVMap(model.Sequence.Shape.Channels).ToArray();
        }
        else
        {
            channels = renderer.getChannelHsvMap().ToArray();
        }

        foreach (var chan in channels)
        {
            ObsChannels.Add(chan);
        }
        SelectedItem = ObsChannels[0];
    }
    
    public void Save()
    {
        _renderer.UpdateChannelHSVMap(ObsChannels);
    }
}