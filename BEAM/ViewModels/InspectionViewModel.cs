using System;
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
    [ObservableProperty] public partial SequenceViewModel currentSequenceViewModel { get; set; }
    
    /// <summary>
    /// The current AnalysisView displayed
    /// </summary>
    public AbstractAnalysisView CurrentAnalysisView
    {
        get =>_currentAnalysisView;
        set => _currentAnalysisView = value;
    }

    /// <summary>
    /// Set the AnalysisView to a default value
    /// </summary>
    private AbstractAnalysisView _currentAnalysisView = new BarPlotAnalysisView();
    
    public string Name { get; } = "Inspect";
    
    public void Update(Rectangle coordRectangle, SequenceViewModel sequence)
    {
        double[] pixelData = sequence.Sequence.GetPixel((long) coordRectangle.BottomRight.Column, (long) coordRectangle.BottomRight.Row);
        _currentAnalysisView.Update(pixelData);
    }
    
    // [RelayCommand]
    // public async Task SetAnalysisView(AbstractAnalysisView abstractAnalysisView)
    // {
    //     abstractAnalysisView = new AnalysisViewPlotBars();
    // }

}