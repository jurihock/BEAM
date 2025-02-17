using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BEAM.Image.Minimap;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Views.Minimap.Popups;

public partial class SequenceMinimapPopupView : Window
{
    public SequenceMinimapPopupView(SequenceViewModel dataBase, SettingsStorer storer)
    {
        InitializeComponent();
        DataContext = new SequenceMinimapPopupViewModel(dataBase, storer);
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
        if (((DataContext as SequenceMinimapPopupViewModel)!).Save())
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
}