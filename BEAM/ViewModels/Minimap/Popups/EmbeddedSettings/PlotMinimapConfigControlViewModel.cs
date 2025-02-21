using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using BEAM.Exceptions;
using BEAM.Image.Minimap;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.Image.Minimap.Utility;
using BEAM.Models.Log;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

/// <summary>
/// View model for configuring plot minimap settings and managing minimap algorithms.
/// </summary>
public partial class PlotMinimapConfigControlViewModel: ViewModelBase
{
    private const String DefaultControlText = "This Algorithm provides no changeable settings";

    private readonly PlotMinimap _plotMinimap;
    
    /// <summary>
    /// Gets or sets the line compaction factor for the minimap.
    /// </summary>
    [ObservableProperty] public partial int LineCompaction { get; set; }
    
    /// <summary>
    /// Gets or sets the currently selected minimap algorithm.
    /// </summary>
    [ObservableProperty] public partial IMinimapAlgorithm? SelectedAlgorithm { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of available minimap algorithms.
    /// </summary>
    [ObservableProperty] public partial ObservableCollection<IMinimapAlgorithm> Algorithms { get; set; } = new ObservableCollection<IMinimapAlgorithm>();
    /// <summary>
    /// Gets or sets the collection of algorithm-specific setting controls.
    /// </summary>
    [ObservableProperty] public partial ObservableCollection<Control> AlgorithmSubSettings { get; set; } = new ObservableCollection<Control>();
    
    
    private SaveUserControl _currentConfigControl = new NullSaveConfig();
    /// <summary>
    /// Saves the current configuration to the plot minimap.
    /// </summary>
    public void Save()
    {
        _plotMinimap.CompactionFactor = LineCompaction;
        _currentConfigControl.Save();
        _plotMinimap.MinimapAlgorithm = SelectedAlgorithm;
        SettingsUtilityHelper<IMinimapAlgorithm>.SetDefaultObject(SelectedAlgorithm);
    }
    
    /// <summary>
    /// Creates a new instance of this view model for the specified plot minimap.
    /// The corresponding algorithm settings  UI element is loaded.
    /// </summary>
    /// <param name="plotMinimap">The <see cref="PlotMinimap"/> whose attributes are being bound to the view.</param>
    public PlotMinimapConfigControlViewModel(PlotMinimap plotMinimap)
    {
        _plotMinimap = plotMinimap;
        LineCompaction = plotMinimap.CompactionFactor;
        SelectedAlgorithm = _plotMinimap.MinimapAlgorithm;
        if (!SettingsUtilityHelper<IMinimapAlgorithm>.ExistAny() )
        {
            TextBlock textBlock = new TextBlock(){Text= "There are no Algorithms to choose from"};
            AlgorithmSubSettings.Add(textBlock);
            return;
        }
        
        
        foreach(var element in SettingsUtilityHelper<IMinimapAlgorithm>.GetDefaultObjects() /*PlotAlgorithmSettingsUtilityHelper.GetDefaultAlgorithms()*/)
        {
            Algorithms.Add(element);
        }

        try
        {
            SelectedAlgorithm = SettingsUtilityHelper<IMinimapAlgorithm>.GetDefaultObject();
        }
        catch (InvalidStateException ex)
        {
            Logger.GetInstance().LogMessage("No valid Plotting algorithm was found + " + ex.Message);
        }
    }

    
    /// <summary>
    /// Handles algorithm selection changes and updates the settings UI.
    /// </summary>
    /// <param name="sender">The source of the selection event.</param>
    /// <param name="e">Event data containing the selected algorithm.</param>
    public void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _currentConfigControl.Save();
        IMinimapAlgorithm? algorithm;
        try
        {
            algorithm = (IMinimapAlgorithm) e.AddedItems[0]!;
        }
        catch (NullReferenceException ex)
        {
            throw new InvalidCastException("The selected Minimap is not a Minimap", ex);
        }
        

        var controls = algorithm.GetSettingsPopupControl();
        AlgorithmSubSettings.Clear();
        if(controls is null)
        {
            _currentConfigControl = new NullSaveConfig();
            AlgorithmSubSettings.Add(new TextBlock() {Text = DefaultControlText});
        }
        else
        {
            _currentConfigControl = controls;
            AlgorithmSubSettings.Add(controls);
        }
    }
}