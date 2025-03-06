using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using BEAM.Models.Log;
using BEAM.Renderer;
using BEAM.ViewModels;

namespace BEAM.Controls;

public class ArgMaxHSVConfigControlViewModel : ViewModelBase, ISaveControl
{
    private readonly ArgMaxRendererColorHSV _renderer;

    public decimal Min { get; set; } = 0;

    public ObservableCollection<ChannelToHSV> ObsChannels
    {
        get;
        set => SetProperty(ref field, value);
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

    public ChannelToHSV SelectedChannel => ObsChannels[SelectedChannelIndex];

    /// <summary>
    /// Displays the color associated with a channel
    /// </summary>
    public IBrush ColorBrush
    {
        get => new SolidColorBrush(SelectedChannel.AvaColor);
    }

    public ArgMaxHSVConfigControlViewModel(ArgMaxRendererColorHSV renderer, SequenceViewModel model)
    {
        ArgumentNullException.ThrowIfNull(renderer);
        ArgumentNullException.ThrowIfNull(model);

        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));

        if (_renderer.GetChannelHsvMap().AmountChannels == 0)
        {
            ObsChannels =
                new ObservableCollection<ChannelToHSV>(new ChannelHSVMap(model.Sequence.Shape.Channels).ToArray());
        }
        else
        {
            ObsChannels = new ObservableCollection<ChannelToHSV>(renderer.GetChannelHsvMap().ToArray());
        }
    }

    public void Save()
    {
        var map = new ChannelHSVMap(ObsChannels.ToArray());
        if (map.GetAmountUsedChannels() == 0)
        {
            // Initiliaze ArgMaxRenderer correctly, if not happened yet
            if (_renderer.GetChannelHsvMap().AmountChannels == 0)
            {
                _renderer.UpdateChannelHSVMap(new ChannelHSVMap(map.AmountChannels).ToArray());
            }
            Logger.GetInstance().Warning(LogEvent.Rendering, "No Channel selected for ArgMaxRenderer.");
            return;
        }

        _renderer.UpdateChannelHSVMap(ObsChannels.ToArray());
    }
}