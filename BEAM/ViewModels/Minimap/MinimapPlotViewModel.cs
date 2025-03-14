using System;
using BEAM.Docking;
using BEAM.Image.Minimap;
using BEAM.ViewModels.Utility;
using BEAM.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScottPlot;

namespace BEAM.ViewModels.Minimap;

/// <summary>
/// View model for managing minimap plot display and interactions.
/// </summary>
public partial class MinimapPlotViewModel : SizeAdjustableViewModelBase, IDockBase
{
    /// <summary>
    /// Gets or sets the current plot being displayed.
    /// </summary>
    [ObservableProperty] private Plot _currentPlot;


    /// <summary>
    /// Gets the display name of the minimap view.
    /// </summary>
    public string Name { get; init; }
    
    private byte _minimapProgress;
    public byte MinimapProgress
    {
        get => _minimapProgress;
        set
        {
            if (_minimapProgress == value) return;
            _minimapProgress = value;
            OnPropertyChanged();
        }
    }
    
    private MinimapProgressWindow ProgressWindow { get; set; }
    private readonly PlotMinimap _source;

    public void OnClose()
    {
        ProgressWindow.Close();
    }

    /// <summary>
    /// Initializes a new instance with the specified plot.
    /// </summary>
    /// <param name="plot">The plot to display.</param>
    /// <param name="source">The minimap based on which the data is being generated.</param>
    /// <param name="name">The name of the sequence corresponding to this minimap</param>
    public MinimapPlotViewModel(Plot plot,  PlotMinimap source, string name = "Minimap View")
    {
        Name = name;
        _currentPlot = plot;
        CurrentPlot = plot;
        this._source = source;
        ProgressWindow = new MinimapProgressWindow(this);
    }
    public void ReplacePlot(Plot newPlot)
    {
        CurrentPlot = newPlot;
    }

    public void InitializeStatusWindow()
    {
        Console.WriteLine("Started new Progress bar");
        MinimapProgress = 0;
        ProgressWindow.Show();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        CurrentPlot.Dispose();
    }
    
    [RelayCommand]
    public void AbortGeneration()
    {
        _source.StopGeneration();
        if (ProgressWindow.IsVisible)
        {
            ProgressWindow.Close();
            ProgressWindow = new MinimapProgressWindow(this);
        }
    }

    public void CloseStatusWindow()
    {
        if (ProgressWindow.IsVisible)
        {
            ProgressWindow.Close();
            ProgressWindow = new MinimapProgressWindow(this);
        }
    }
}
