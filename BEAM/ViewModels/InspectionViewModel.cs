using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BEAM.Analysis;
using BEAM.Datatypes;
using BEAM.Docking;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScottPlot;
using System.Collections.Specialized;
using System.Threading.Tasks;
using NP.Utilities;


namespace BEAM.ViewModels;

/// <summary>
/// View model controlling the inspection dock.
/// </summary>
public partial class InspectionViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] private Plot? _currentPlot;
    private bool KeepData { get; set; }

   
    private SequenceViewModel _currentSequenceViewModel;
    private Analysis.Analysis _currentAnalysis;
    private (Coordinate2D pressed, Coordinate2D released) _pointerRectanglePosition;
    public ObservableCollection<SequenceViewModel> ExistingSequenceViewModels { get; private set;  } = new();
    
    public static List<Analysis.Analysis> AnalysisList { get;  } =
    [
        new PixelAnalysisChannel(),
        new CirclePlot(),
        new RegionAnalysisStandardDeviationOfChannels(),
        new RegionAnalysisAverageOfChannels()
    ];
    
    public InspectionViewModel(SequenceViewModel sequenceViewModel, DockingViewModel dock)
    {
        _currentAnalysis = AnalysisList[0];
        _currentSequenceViewModel = sequenceViewModel;
        dock.Items.CollectionChanged += DockingItemsChanged;

        foreach (var item in dock.Items)
        {
            if (item is SequenceViewModel model)
            {
                ExistingSequenceViewModels.Add(model);
            }
        }
    }
    

    public string Name { get; } = "Inspection Window";
    public void OnClose()
    {
        _currentSequenceViewModel.UnregisterInspectionViewModel(this);
    }

    /// <summary>
    /// When the user interacted with the view, the coordinates of where the
    /// pointer was pressed and released, are passed to this method.
    /// </summary>
    public void Update(Coordinate2D pressedPoint, Coordinate2D releasedPoint)
    {
        if (KeepData) return;
        _pointerRectanglePosition = (pressedPoint, releasedPoint);
        Plot result = _currentAnalysis.Analyze(pressedPoint, releasedPoint, _currentSequenceViewModel.Sequence);
        CurrentPlot = result;
    }
    
    
    /// <summary>
    /// Creates a new Inspection window
    /// </summary>
    [RelayCommand]
    public void Clone()
    {
        _currentSequenceViewModel.OpenInspectionViewCommand.Execute(null);
    }
    

    
    /// <summary>
    /// When called, this method changes the currently used analysis method.
    /// </summary>
    /// <param name="index">The index of the new analysis mode</param>
    [RelayCommand]
    public Task ChangeAnalysis(int index)
    {
        if (ExistingSequenceViewModels.IsNullOrEmpty())
        {
            CurrentPlot = PlotCreator.CreatePlaceholderPlot();
            return Task.FromResult(false);
        }

        _currentAnalysis = AnalysisList[index];
        CurrentPlot = _currentAnalysis.Analyze(
            _pointerRectanglePosition.pressed,
            _pointerRectanglePosition.released,
            _currentSequenceViewModel.Sequence);
        return Task.CompletedTask;
    }

    /// <summary>
    /// This one will change the currently selected sequence to a new one.
    /// </summary>
    /// <param name="index">The index of the new sequence to be used</param>
    [RelayCommand]
    public void ChangeSequence(int index)
    {
        if(index < 0 || index >= ExistingSequenceViewModels.Count) return;
        _currentSequenceViewModel.UnregisterInspectionViewModel(this);
        _currentSequenceViewModel = ExistingSequenceViewModels[index];
        _currentSequenceViewModel.RegisterInspectionViewModel(this);
    }
    
    
    /// <summary>
    /// When the amount of docks registered by the DockingViewModel changes, this method will be called.
    /// If necessary, the list of existing SequenceVieModels will be updated.
    /// </summary>
    /// <param name="sender">the sender of the event</param>
    /// <param name="e">notification parameters</param>
    private void DockingItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is not SequenceViewModel sequenceViewModel) continue;
                ExistingSequenceViewModels.Add(sequenceViewModel);
                if(ExistingSequenceViewModels.Count == 1) SwitchToFirst();
            }
            
        }

        if (e.OldItems is null) return;
        {
            foreach (var item in e.OldItems)
            {
                if (item is not SequenceViewModel sequenceViewModel) continue;
                ExistingSequenceViewModels.Remove(sequenceViewModel);
                if (sequenceViewModel == _currentSequenceViewModel) SwitchToFirst();
            }
        }
    }
    
    

    
    /// <summary>
    /// This method will simply switch to the first sequence in the list of existing sequences.
    /// </summary>
    private void SwitchToFirst()
    {
        if (ExistingSequenceViewModels.Count == 0)
        {
            CurrentPlot = PlotCreator.CreatePlaceholderPlot();
            return;
        }
        _currentSequenceViewModel.UnregisterInspectionViewModel(this);
        _currentSequenceViewModel = ExistingSequenceViewModels[0];
        _currentSequenceViewModel.RegisterInspectionViewModel(this);
    }
    
    /// <summary>
    /// This method will update the new data acceptance.
    /// </summary>
    /// <param name="isChecked"></param>
    public void CheckBoxChanged(bool? isChecked)
    {
        KeepData = isChecked ?? false;
    }

    public int CurrentSequenceIndex()
    {
        return ExistingSequenceViewModels.FindIdx(_currentSequenceViewModel);
    }
    
    public int CurrentAnalysisIndex()
    {
        return AnalysisList.FindIdx(_currentAnalysis);
    }

    public void Dispose()
    {
        if (CurrentPlot is null) return;
        CurrentPlot.Dispose();
        GC.SuppressFinalize(this);
    }
}