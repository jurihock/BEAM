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

public partial class PlotMinimapConfigControlViewModel: ViewModelBase
{
    private const String DefaultControlText = "This Algorithm provides no changeable settings";

    private readonly PlotMinimap _plotMinimap;
    [ObservableProperty] public partial int LineCompaction { get; set; }
    
    [ObservableProperty] public partial IMinimapAlgorithm SelectedAlgorithm { get; set; }
    [ObservableProperty] public partial ObservableCollection<IMinimapAlgorithm> Algorithms { get; set; } = new ObservableCollection<IMinimapAlgorithm>();
    [ObservableProperty] public partial ObservableCollection<Control> AlgorithmSubSettings { get; set; } = new ObservableCollection<Control>();
    
    
    private ISaveControl _currentConfigControl = new NullSaveConfig();
    public void Save()
    {
        _plotMinimap.CompactionFactor = LineCompaction;
        _currentConfigControl.Save();
        _plotMinimap.MinimapAlgorithm = SelectedAlgorithm;
        PlotAlgorithmSettingsUtilityHelper.SetDefaultAlgorithm(SelectedAlgorithm);
    }
    
    
    public PlotMinimapConfigControlViewModel(PlotMinimap plotMinimap)
    {
        _plotMinimap = plotMinimap;
        LineCompaction = plotMinimap.CompactionFactor;
        SelectedAlgorithm = _plotMinimap.MinimapAlgorithm;
        if (!PlotAlgorithmSettingsUtilityHelper.ExistAny())
        {
            TextBlock textBlock = new TextBlock(){Text= "There are no Algorithms to choose from"};
            AlgorithmSubSettings.Add(textBlock);
            return;
        }
        
        
        foreach(var element in PlotAlgorithmSettingsUtilityHelper.GetDefaultAlgorithms())
        {
            Algorithms.Add(element);
        }

        try
        {
            SelectedAlgorithm = PlotAlgorithmSettingsUtilityHelper.GetDefaultAlgorithm();
        }
        catch (InvalidStateException ex)
        {
            Logger.GetInstance().LogMessage("No valid Plotting algorithm was found + " + ex.Message);
        }
    }

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