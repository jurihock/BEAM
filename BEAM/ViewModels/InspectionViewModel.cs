using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using BEAM.Analysis;
using BEAM.Datatypes;
using BEAM.Docking;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NP.Utilities;
using ScottPlot;
using ShimSkiaSharp;


namespace BEAM.ViewModels;

/// <summary>
/// View model controlling the inspection dock.
/// </summary>
public partial class InspectionViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] private Plot _currentPlot = null!;
    [ObservableProperty] private bool _keepData = false;
    private SequenceViewModel _currentSequenceViewModel;
    private Analysis.Analysis _currentAnalysis;
    private (Coordinate2D pressed, Coordinate2D released) _pointerRectanglePosition;
    private Plot PlaceholderPlot { get; set; }
    public ObservableCollection<SequenceViewModel> ExistingSequenceViewModels { get; private set; } = new();

    public static List<Analysis.Analysis> AnalysisList { get; } = new()
    {
        new PixelAnalysisChannel(),
        new CirclePlot(),
        new RegionAnalysisStandardDeviationOfChannels()
    };

    public InspectionViewModel(SequenceViewModel sequenceViewModel)
    {
        _currentAnalysis = AnalysisList[0];
        _currentSequenceViewModel = sequenceViewModel;
        ExistingSequenceViewModels.Add(sequenceViewModel);
        _currentSequenceViewModel.DockingVm.Items.CollectionChanged += DockingItemsChanged!;
        PlaceholderPlot = _CreatePlaceholderPlot();
    }


    public string Name { get; } = "Inspection Window";

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
    /// When called, this method woll change the currently used analysis method.
    /// </summary>
    /// <param name="index">The index of the new analysis mode</param>
    [RelayCommand]
    public void ChangeAnalysis(int index)
    {
        if (ExistingSequenceViewModels.IsNullOrEmpty()) return;
        _currentAnalysis = AnalysisList[index];
        CurrentPlot = _currentAnalysis.Analyze(
            _pointerRectanglePosition.pressed,
            _pointerRectanglePosition.released,
            _currentSequenceViewModel.Sequence);
    }

    /// <summary>
    /// This one will change the currently selected sequence to a new one.
    /// </summary>
    /// <param name="index">The index of the new sequence to be used</param>
    [RelayCommand]
    public void ChangeSequence(int index)
    {
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
    private void DockingItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is not SequenceViewModel sequenceViewModel) continue;
                ExistingSequenceViewModels.Add(sequenceViewModel);
                if (ExistingSequenceViewModels.Count == 1) SwitchToFirst();
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
    /// This method will creat a placeholder plot that will be displayed when no sequence is selected.
    /// </summary>
    /// <returns></returns>
    private Plot _CreatePlaceholderPlot()
    {
        Plot myPlot = new();

        Coordinates center = new(0, 0);
        double radiusX = 1;
        double radiusY = 5;

        for (int i = 0; i < 5; i++)
        {
            float angle = (i * 20);
            var el = myPlot.Add.Ellipse(center, radiusX, radiusY, angle);
            el.LineWidth = 3;
            el.LineColor = Colors.Blue.WithAlpha(0.1 + 0.2 * i);
        }

        myPlot.Layout.Frameless();
        myPlot.Axes.Margins(0, 0);
        myPlot.Title("No sequence selected");
        return myPlot;
    }

    /// <summary>
    /// This method will simply switch to the first sequence in the list of existing sequences.
    /// </summary>
    private void SwitchToFirst()
    {
        if (ExistingSequenceViewModels.Count == 0)
        {
            CurrentPlot = PlaceholderPlot;
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
    [RelayCommand]
    public void CheckBoxChanged(bool? isChecked)
    {
        KeepData = isChecked ?? false;
    }

    public void Dispose()
    {
        CurrentPlot.Dispose();
        PlaceholderPlot.Dispose();
        GC.SuppressFinalize(this);
    }
}