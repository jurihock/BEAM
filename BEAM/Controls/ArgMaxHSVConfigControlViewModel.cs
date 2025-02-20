using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BEAM.Datatypes.Color;
using BEAM.Renderer;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

public partial class ArgMaxHSVConfigControlViewModel : ViewModelBase, ISaveControl
{
    private readonly ArgMaxRendererColorHSV _renderer;

    public ObservableCollection<ChannelToHSV> TestData
    {
        get; 
        set; 
    } = [];
    
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
        
        var testData = new List<ChannelToHSV>
        {
            new ChannelToHSV(0),
            new ChannelToHSV(1),
            new ChannelToHSV(2),
        };
        //TestData = new ObservableCollection<ChannelToHSV>(testData);
        
    }
    
    public void Save()
    {
        _renderer.UpdateChannelHSVMap(ObsChannels);
    }
}