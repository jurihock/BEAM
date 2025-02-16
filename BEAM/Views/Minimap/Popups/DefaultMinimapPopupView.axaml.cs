using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BEAM.Docking;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Views.Minimap.Popups;

public partial class DefaultMinimapPopupView : Window, IDockBase
{
    public DefaultMinimapPopupView()
    {
        InitializeComponent();
        var vm = new DefaultMinimapPopupViewModel();
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
        var elements = this.Get<ComboBox>("MinimapSelector");
        foreach (var elementsItem in elements.Items)
        {
            if (elementsItem is null)
            {
                continue;
            }
            
        }
        if (((DataContext as DefaultMinimapPopupViewModel)!).Save())
        {
            Close();
        }
        // If execution is here -> Save failed, hints in controls
    }

    public new string Name { get; } = "Configure Default Minimap settings";

    public void OnClose()
    {
        return;
    }

    private void MinimapSelector_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = (DataContext as DefaultMinimapPopupViewModel);
        if (vm is null)
        {
            return;
        }
        vm.SelectionChanged(sender, e);
    }
}