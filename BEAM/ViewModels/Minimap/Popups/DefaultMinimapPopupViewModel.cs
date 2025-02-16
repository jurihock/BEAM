using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using BEAM.Image.Minimap;
using BEAM.Renderer;
using NP.Utilities;

namespace BEAM.ViewModels.Minimap.Popups;

public class DefaultMinimapPopupViewModel
{
    public ObservableCollection<Control> RendererSelectionControls { get; } = [];
    
    private List<ISaveControl> _controls = [];
    
    private SequenceViewModel _sequenceViewModel;

    private int _selection;
    
    public DefaultMinimapPopupViewModel(SequenceViewModel sequenceViewModel)
    {
        _sequenceViewModel = sequenceViewModel;

        _selection = MinimapSettingsUtilityHelper;

        for (var index = 0; index < sequenceViewModel.Renderers.Length; index++)
        {
            var renderer = sequenceViewModel.Renderers[index];
            RendererSelectionControls.Add(_BuildSelectionButton(renderer, index));
            RendererSelectionControls.Add(_BuildPanel(renderer));
        }
    }
    
    public bool Save()
    {
        foreach (var control in _controls)
        {
            control.Save();
        }

        foreach (var renderer in _sequenceViewModel.Renderers)
        {
            renderer.MinimumOfIntensityRange = (double) Min;
            renderer.MaximumOfIntensityRange = (double) Max;
        }

        _sequenceViewModel.RendererSelection = _selection;
        _sequenceViewModel.RenderersUpdated.Invoke(this, new RenderersUpdatedEventArgs());
        return true;
    }
    private StackPanel _BuildPanel(SequenceRenderer renderer)
    {
        if (_mapping is null)
        {
            CreateMapping();
        }
        var panel = new StackPanel() { Margin = new Thickness(30, 0, 0, 0) };

        switch (renderer)
        {
            case HeatMapRenderer htmRenderer:
                var hmView = new HeatMapConfigControlView(htmRenderer, _sequenceViewModel);
                panel.Children.Add(hmView);
                _controls.Add(hmView);
                break;
            case ChannelMapRenderer chmRenderer:
                var chmView = new ChannelMapConfigControlView(chmRenderer, _sequenceViewModel);
                panel.Children.Add(chmView);
                _controls.Add(chmView);
                break;
            case ArgMaxRenderer:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(renderer));
        }

        return panel;
    }
    
}

public interface ISaveControl
{
    public void Save();
}
