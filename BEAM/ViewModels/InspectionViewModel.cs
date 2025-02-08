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
    [ObservableProperty] private bool _keepData  = false;

    
    private SequenceViewModel _currentSequenceViewModel;
    private Analysis.Analysis _currentAnalysis;
    private (Coordinate2D pressed, Coordinate2D released) _pointerRectanglePosition;

    public List<Analysis.Analysis> AnalysisList { get; private set;  } = new()
    {
        new PixelAnalysisChannel(),
        new CirclePlot()
    };

    public List<SequenceViewModel> ExistingSequenceViewModels { get; set; } = new();

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
        _currentAnalysis = AnalysisList[0];
        _currentSequenceViewModel = sequenceViewModel;
        ExistingSequenceViewModels.Add(sequenceViewModel);
        _currentSequenceViewModel.DockingVm.Items.CollectionChanged += DockingItemsChanged;
    }
    

    public string Name { get; } = "Inspection Window";
    public void Update(Coordinate2D pressedPoint, Coordinate2D releasedPoint)
    {
        if(_keepData) return;
        _pointerRectanglePosition = (pressedPoint, releasedPoint);
        Plot result = _currentAnalysis.Analyze(pressedPoint, releasedPoint, _currentSequenceViewModel.Sequence);
        CurrentPlot = result;
    }
    
    [RelayCommand]
    public async Task Clone()
    {
        _currentSequenceViewModel.OpenInspectionViewCommand.Execute(null);
    }
    
    [RelayCommand]
    public Task ChangeAnalysis(int index)
    {
        _currentAnalysis = AnalysisList[index];
        Console.WriteLine("Changed Analysis to: " + _currentAnalysis);
        _currentPlot = _currentAnalysis.Analyze(_pointerRectanglePosition.pressed, _pointerRectanglePosition.released, _currentSequenceViewModel.Sequence);
        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ChangeSequence(int index)
    {
        _currentSequenceViewModel.UnregisterInspectionViewModel(this);
        _currentSequenceViewModel = ExistingSequenceViewModels[index];
        _currentSequenceViewModel.RegisterInspectionViewModel(this);
        Console.WriteLine("Changed Sequence to: " + _currentSequenceViewModel);
    }
    
    private void DockingItemsChanged(object sender, EventArgs e)
    {
        var dockingvm = _currentSequenceViewModel.DockingVm;
        ExistingSequenceViewModels.Clear();
        foreach (var item in dockingvm.Items)
        {
            if (item is SequenceViewModel sequenceViewModel)
            {
                ExistingSequenceViewModels.Add(sequenceViewModel);
            }
        }
    }
    
    [RelayCommand]
    public async Task CheckBoxChanged(bool? isChecked)
    {
        _keepData = isChecked ?? false;
        Console.WriteLine("Now data is accepted is: " + isChecked);
    }

    // [RelayCommand]
    // public async Task SetAnalysisView(AbstractAnalysisView abstractAnalysisView)
    // {
    //     abstractAnalysisView = new AnalysisViewPlotBars();
    // }
}