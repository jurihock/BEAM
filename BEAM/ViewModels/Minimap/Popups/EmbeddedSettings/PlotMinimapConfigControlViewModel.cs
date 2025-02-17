using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using BEAM.Exceptions;
using BEAM.Image.Minimap;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.Image.Minimap.Utility;
using BEAM.Log;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

public partial class PlotMinimapConfigControlViewModel: ViewModelBase
{
    private static String _defaultControlText = "This Algorithm provides no changeable settings";

    private PlotMinimap _plotMinimap;
    [ObservableProperty] public int lineCompaction;
    
    [ObservableProperty] public partial IMinimapAlgorithm SelectedAlgorithm { get; set; }
    [ObservableProperty] public partial ObservableCollection<IMinimapAlgorithm> algorithms { get; set; } = new ObservableCollection<IMinimapAlgorithm>();
    [ObservableProperty] public partial ObservableCollection<Control> algorithmSubSettings { get; set; } = new ObservableCollection<Control>();
    
    private ISaveControl _currentControl = new NullSaveConfig();
    
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
        lineCompaction = plotMinimap.CompactionFactor;
        if (!MinimapSettingsUtilityHelper.ExistAny())
        {
            TextBlock textBlock = new TextBlock(){Text= "There are no Minimaps to choose from"};
            return;
        }
        
        
        foreach(var element in PlotAlgorithmSettingsUtilityHelper.GetDefaultAlgorithms())
        {
            algorithms.Add(element);
        }

        try
        {
            SelectedAlgorithm = PlotAlgorithmSettingsUtilityHelper.GetDefaultAlgorithm();
        }
        catch (InvalidStateException ex)
        {
            Logger.GetInstance().LogMessage("No valid Plotting algorithm was found");
        }
        
        
    }

    public void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _currentControl.Save();
        IMinimapAlgorithm? algorithm;
        try
        {
            algorithm = (IMinimapAlgorithm) e.AddedItems[0]!;
        }
        catch (NullReferenceException ex)
        {
            throw new InvalidCastException("The selected Minimap is not a Minimap", ex);
        }

        //TODO: Alternatively one minimap only ever has one Control Window and we can get access to it thathw ay
        //TODO: Alternatively one ([User]Control, ISaveControl) inherits from the other
        var controls = algorithm.GetSettingsPopupControl();
        algorithmSubSettings.Clear();
        if(controls is null)
        {
            _currentControl = new NullSaveConfig();
            algorithmSubSettings.Add(new TextBlock() {Text = _defaultControlText});
        }
        else
        {
            _currentControl = controls;
            algorithmSubSettings.Add(controls);
        }
    }
    
}