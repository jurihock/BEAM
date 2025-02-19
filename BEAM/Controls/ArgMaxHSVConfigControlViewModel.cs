using System;
using System.Data;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using BEAM.Datatypes.Color;
using BEAM.Renderer;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

public partial class ArgMaxHSVConfigControlViewModel
    : ViewModelBase, ISaveControl
{
    private static readonly HueColorLut hcl = new HueColorLut();
    private ArgMaxRendererColorHSV renderer;
    [ObservableProperty] private partial ChannelHSVMap ChannelHsvMap { get; set; }
    
    public FlatTreeDataGridSource<ChannelToHSV> ChannelSource { get; }

    public ArgMaxHSVConfigControlViewModel(ArgMaxRendererColorHSV renderer, SequenceViewModel model)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (model == null) throw new ArgumentNullException(nameof(model));
        this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));

        if (this.renderer.getChannelHsvMap().AmountChannels == 0)
        {
            ChannelHsvMap = new ChannelHSVMap(model.Sequence.Shape.Channels);
        }
        else
        {
            ChannelHsvMap = renderer.getChannelHsvMap();
        }
        
        ChannelSource = new FlatTreeDataGridSource<ChannelToHSV>(ChannelHsvMap.ToArray())
        {
            Columns =
            {
                new TextColumn<ChannelToHSV, int>
                    ("Index", x => x.Index),
                new CheckBoxColumn<ChannelToHSV>
                    ("Used", x => x.IsUsedForArgMax),
            },
        };
    }
    
    public void Save()
    {
        renderer.UpdateChannelHSVMap(ChannelHsvMap);
        ChannelHsvMap = renderer.getChannelHsvMap();
    }
}