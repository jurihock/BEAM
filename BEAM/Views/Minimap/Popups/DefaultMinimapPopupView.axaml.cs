using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Views.Minimap.Popups;

public partial class DefaultMinimapPopupView : Window
{
    public DefaultMinimapPopupView(SequenceViewModel sequenceVm)
    {
        InitializeComponent();

        DataContext = new DefaultMinimapPopupViewModel(sequenceVm);
        AddHandler(KeyDownEvent, (_, e) =>
        {
            if (e.Key == Key.Escape) Close();
        });
        DataContextChanged += OnDataContextChanged;
    }
    
    
    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    private void OnDataContextChanged(object? sender, EventArgs eventArgs)
    {
    }


    private void Disable(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as DefaultMinimapPopupViewModel;
        if (vm is null)
        {
            return;
        }

        vm.DisableMinimap();
    }
    private void TrySave(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as DefaultMinimapPopupViewModel)!).Save())
        {
            Close();
        }
    }

    public new string Name { get; } = "Configure Default Minimap settings";
    

    private void MinimapSelector_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = (DataContext as DefaultMinimapPopupViewModel);
        if (vm is null)
        {
            return;
        }
        vm.SelectionChanged(sender, e);
    }

    private void RenderMinimap(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as DefaultMinimapPopupViewModel)!).Save())
        {
            Close();
        }
        var vm = DataContext as DefaultMinimapPopupViewModel;
        if (vm == null)
        {
            return;
        }
        vm.RenderMinimap();
        Close();
    }
}