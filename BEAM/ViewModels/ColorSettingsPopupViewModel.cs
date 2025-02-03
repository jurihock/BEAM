// (c) Paul Stier, 2025

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using BEAM.Controls;
using BEAM.Renderer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class ColorSettingsPopupViewModel : ViewModelBase
{
    public ObservableCollection<Control> RendererSelectionControls { get; } = [];
    private SequenceViewModel _sequenceViewModel;

    private SequenceRenderer[] _editedRenderers;

    private int _selection;

    public ColorSettingsPopupViewModel(SequenceViewModel sequenceViewModel)
    {
        _sequenceViewModel = sequenceViewModel;
        _editedRenderers = _sequenceViewModel.Renderers.Select(r => (SequenceRenderer)r.Clone()).ToArray();

        _selection = _sequenceViewModel.RendererSelection;

        for (var index = 0; index < _editedRenderers.Length; index++)
        {
            var renderer = _editedRenderers[index];
            RendererSelectionControls.Add(_BuildSelectionButton(renderer, index));
            RendererSelectionControls.Add(_BuildPanel(renderer));
        }
    }

    private RadioButton _BuildSelectionButton(SequenceRenderer renderer, int index)
    {
        var button = new RadioButton()
        {
            GroupName = "RendererSelectionGroup",
            Content = renderer.GetName(),
            IsChecked = (_selection == index),
        };
        button.IsCheckedChanged += (s, e) =>
        {
            var btn = (RadioButton)s!;
            if (btn.IsChecked ?? false) _selection = index;
        };

        return button;
    }

    private static StackPanel _BuildPanel(SequenceRenderer renderer)
    {
        var panel = new StackPanel() { Margin = new Thickness(30, 0, 0, 0) };

        switch (renderer)
        {
            case HeatMapRenderer htmRenderer:
                panel.Children.Add(new HeatMapConfigControlView(htmRenderer));
                break;
            case ChannelMapRenderer chmRenderer:
                panel.Children.Add(new ChannelMapConfigControlView(chmRenderer));
                break;
            case ArgMaxRenderer:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(renderer));
        }

        return panel;
    }

    [RelayCommand]
    private void Save()
    {
        _sequenceViewModel.RendererSelection = _selection;
        _sequenceViewModel.Renderers = _editedRenderers;
    }
}