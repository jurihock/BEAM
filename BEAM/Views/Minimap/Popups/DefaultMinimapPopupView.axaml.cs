using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Views.Minimap.Popups;

/// <summary>
/// Window for configuring and managing default minimap settings.
/// Provides options to select, disable, and customize minimap behavior.
/// </summary>
public partial class DefaultMinimapPopupView : Window
{
    /// <summary>
    /// Initializes a new instance of the DefaultMinimapPopupView with the specified sequence view model.
    /// </summary>
    /// <param name="sequenceVm">The sequence view model to configure.</param>
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

    /// <summary>
    /// Handles the window close action.
    /// Invoked through the corresponding button, not the closing x in the corner.
    /// </summary>
    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    private void OnDataContextChanged(object? sender, EventArgs eventArgs)
    {
    }


    /// <summary>
    /// Disables the current minimap through the view model.
    /// </summary>
    private void Disable(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as DefaultMinimapPopupViewModel;
        if (vm is null)
        {
            return;
        }

        vm.DisableMinimap();
    }


    /// <summary>
    /// Attempts to save the current minimap configuration.
    /// </summary>
    private void TrySave(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as DefaultMinimapPopupViewModel)!).Save())
        {
            Close();
        }
    }

    public new string Name { get; } = "Configure Default Minimap settings";


    /// <summary>
    /// Handles minimap selection changes and updates the UI accordingly.
    /// </summary>
    private void MinimapSelector_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = (DataContext as DefaultMinimapPopupViewModel);
        if (vm is null)
        {
            return;
        }
        vm.SelectionChanged(sender, e);
    }

    /// <summary>
    /// Renders the selected minimap configuration and closes the window.
    /// </summary>
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