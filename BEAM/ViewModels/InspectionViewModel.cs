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
    private SequenceViewModel _currentSequenceViewModel;
    private IPixelAnalysis _currentPixelAnalysis;
    public List<IPixelAnalysis> AnalysisList { get; } = new List<IPixelAnalysis>
    {
        new PixelAnalysisChannel(),
        new HistogramAnalysis()
    };

    public List<SequenceViewModel> ExistingSequenceViewModels { get; } = new List<SequenceViewModel>();

    /// <summary>
    /// The current AnalysisView displayed
    /// </summary>
    // public AbstractAnalysisView CurrentAnalysisView
    // {
    //     get => _currentAnalysisView;
    //     set => _currentAnalysisView = value;
    // }

    public InspectionViewModel(SequenceViewModel sequenceViewModel)
    {
        _currentPixelAnalysis = AnalysisList[0];
        _currentSequenceViewModel = sequenceViewModel;
        ExistingSequenceViewModels.Add(sequenceViewModel);
    }

    public string Name { get; } = "Inspect";
    public void OnClose()
    {
        throw new NotImplementedException();
    }

    public void Update(Rectangle coordRectangle, SequenceViewModel sequence)
    {
        
    }
    
    public void Update(Coordinate2D point, SequenceViewModel sequenceViewModel)
    {
        Plot result = _currentPixelAnalysis.analysePixel(sequenceViewModel.Sequence, point);
        CurrentPlot = result;
    }
    
    [RelayCommand]
    public async Task Clone()
    {
        _currentSequenceViewModel.OpenInspectionViewCommand.Execute(null);
    }
    
    [RelayCommand]
    public async Task ChangeAnalysis(int index)
    {
        if(index >= AnalysisList.Count) return;
        Console.WriteLine("Changed Analysis to: " + AnalysisList[index]);
        _currentPixelAnalysis = AnalysisList[index];
    }
    
    // [RelayCommand]
    // public async Task SetAnalysisView(AbstractAnalysisView abstractAnalysisView)
    // {
    //     abstractAnalysisView = new AnalysisViewPlotBars();
    // }
}