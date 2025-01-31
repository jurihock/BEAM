using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.Views.AnalysisView;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScottPlot;


namespace BEAM.ViewModels;

public partial class InspectionViewModel : ViewModelBase, IDockBase
{
    // [ObservableProperty] public partial SequenceViewModel currentSequenceViewModel { get; set; }
    
    public ObservableCollection<AbstractAnalysisView> openAnalysisViews { get; set; }

    /// <summary>
    /// Set the AnalysisView to a default value
    /// </summary>
    // private AbstractAnalysisView _currentAnalysisView = new BarPlotAnalysisView();

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
        openAnalysisViews =
        [
            new CoordinateAnalysisView(),
            new BarPlotAnalysisView(),
            new BarPlotAnalysisView()
        ];
    }

    public string Name { get; } = "Inspect";

    public void Update(Rectangle coordRectangle, SequenceViewModel sequence)
    {
        foreach (var analysisView in openAnalysisViews)
        {
            analysisView.Update(coordRectangle, sequence.Sequence);
        }
    }
    
    // [RelayCommand]
    // public async Task SetAnalysisView(AbstractAnalysisView abstractAnalysisView)
    // {
    //     abstractAnalysisView = new AnalysisViewPlotBars();
    // }
}