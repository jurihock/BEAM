using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using BEAM.Analysis;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.ImageSequence;
using BEAM.Views;
using BEAM.Views.AnalysisView;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScottPlot;
using ScottPlot.Plottables;
using Rectangle = BEAM.Datatypes.Rectangle;


namespace BEAM.ViewModels;

public partial class InspectionViewModel : ViewModelBase, IDockBase
{
    // [ObservableProperty] public partial SequenceViewModel currentSequenceViewModel { get; set; }

    [ObservableProperty] private Plot _currentPlot;
    private Sequence _currentSequence;
    private IPixelAnalysis _currentPixelAnalysis;

    /// <summary>
    /// The current AnalysisView displayed
    /// </summary>
    // public AbstractAnalysisView CurrentAnalysisView
    // {
    //     get => _currentAnalysisView;
    //     set => _currentAnalysisView = value;
    // }

    public InspectionViewModel()
    {
        _currentPixelAnalysis = new PixelAnalysisChannel();
    }

    public string Name { get; } = "Inspect";

    public void Update(Rectangle coordRectangle, SequenceViewModel sequence)
    {
        
    }
    
    public void Update(Coordinate2D point, SequenceViewModel sequenceViewModel)
    {
        Console.WriteLine("ClickUpdated Arrived in InspectionViewModel");

        Plot result = _currentPixelAnalysis.analysePixel(sequenceViewModel.Sequence, point);
        CurrentPlot = result;
    }
    
    // [RelayCommand]
    // public async Task SetAnalysisView(AbstractAnalysisView abstractAnalysisView)
    // {
    //     abstractAnalysisView = new AnalysisViewPlotBars();
    // }
}