using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BEAM.Docking;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Views.Minimap.Popups;

public partial class DefaultMinimapPopupView : Window
{
    public DefaultMinimapPopupView(SequenceViewModel sequencVm)
    {
        InitializeComponent();
        var vm = new DefaultMinimapPopupViewModel(sequencVm);
        DataContext = vm;
        
        this.DataContextChanged += OnDataContextChanged;
    }
    
    
    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    private void OnDataContextChanged(object? sender, EventArgs eventArgs)
    {
        return;
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