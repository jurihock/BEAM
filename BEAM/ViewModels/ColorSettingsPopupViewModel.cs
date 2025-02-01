// (c) Paul Stier, 2025

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using BEAM.Renderer;
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
        return button;
    }

    private StackPanel _BuildPanel(SequenceRenderer renderer)
    {
        var panel = new StackPanel() { Margin = new Thickness(30, 0, 0, 0) };

        switch (renderer.GetRenderType())
        {
            case RenderTypes.HeatMapRendererRb:
                _FillHeatmapRenderer(panel, (renderer as HeatMapRendererRB)!);
                break;
            case RenderTypes.ChannelMapRenderer:
                _FillChanelMapRenderer(panel, (renderer as ChannelMapRenderer)!);
                break;
            case RenderTypes.ArgMaxRendererGrey:
                _FillArgMaxRenderer(panel, (renderer as ArgMaxRendererGrey)!);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return panel;
    }

    private void _ChannelMapRendererChannelInput(StackPanel panel, SequenceRenderer renderer, string channelName,
        string propPath)
    {
        var channelPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 5),
        };
        var input = new TextBox() { VerticalAlignment = VerticalAlignment.Center };
        var binding = new Binding() { Source = renderer, Path = propPath };
        input.Bind(TextBox.TextProperty, binding);
        channelPanel.Children.Add(input);

        channelPanel.Children.Add(new TextBlock()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0),
            Text = $"-> {channelName}"
        });

        panel.Children.Add(channelPanel);
    }

    private void _FillChanelMapRenderer(StackPanel panel, ChannelMapRenderer renderer)
    {
        _ChannelMapRendererChannelInput(panel, renderer, "Red", nameof(renderer.ChannelRed));
        _ChannelMapRendererChannelInput(panel, renderer, "Green", nameof(renderer.ChannelGreen));
        _ChannelMapRendererChannelInput(panel, renderer, "Blue", nameof(renderer.ChannelBlue));
    }

    private void _FillHeatmapRenderer(StackPanel panel, HeatMapRenderer renderer)
    {
        panel.Orientation = Orientation.Horizontal;
        panel.Children.Add(new TextBlock()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0),
            Text = "Channel:"
        });

        var input = new TextBox()
        {
            VerticalAlignment = VerticalAlignment.Center,
        };
        var binding = new Binding()
        {
            Source = renderer,
            Path = nameof(renderer.Channel)
        };
        input.Bind(TextBox.TextProperty, binding);
        panel.Children.Add(input);
    }

    private void _FillArgMaxRenderer(StackPanel panel, ArgMaxRenderer renderer)
    {
    }

    [RelayCommand]
    private void Save()
    {
        _sequenceViewModel.RendererSelection = _selection;
        _sequenceViewModel.Renderers = _editedRenderers;
    }
}