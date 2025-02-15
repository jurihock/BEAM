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

public partial class InspectionViewModel : ViewModelBase, IDockBase
{
    // [ObservableProperty] public partial SequenceViewModel currentSequenceViewModel { get; set; }

    [ObservableProperty] private Plot _currentPlot;
    [ObservableProperty] private bool _keepData  = false;
    private SequenceViewModel _currentSequenceViewModel;
    private Analysis.Analysis _currentAnalysis;
    private (Coordinate2D pressed, Coordinate2D released) _pointerRectanglePosition;
    
    private Plot PlaceholderPlot { get; set; }

    
    public ObservableCollection<SequenceViewModel> ExistingSequenceViewModels { get; private set;  } = new();
    public List<Analysis.Analysis> AnalysisList { get;  } = new()
    {
        new PixelAnalysisChannel(),
        new CirclePlot(),
        new RegionAnalysisStandardDeviationOfChannels()
    };


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
        PlaceholderPlot = createPlaceholderPlot();
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
        _currentPlot = _currentAnalysis.Analyze(_pointerRectanglePosition.pressed, 
            _pointerRectanglePosition.released, 
            _currentSequenceViewModel.Sequence);
        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ChangeSequence(int index)
    {
        _currentSequenceViewModel.UnregisterInspectionViewModel(this);
        _currentSequenceViewModel = ExistingSequenceViewModels[index];
        _currentSequenceViewModel.RegisterInspectionViewModel(this);
        Console.WriteLine("Changed Sequence to: " + _currentSequenceViewModel.Name);
    }
    
    private void DockingItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
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
    
    private Plot createPlaceholderPlot()
    {
        Plot myPlot = new();
        
        Coordinates center = new(0, 0);
        double radiusX = 1;
        double radiusY = 5;

        for (int i = 0; i < 5; i++)
        {
            float angle =(i * 20);
            var el = myPlot.Add.Ellipse(center, radiusX, radiusY, angle);
            el.LineWidth = 3;
            el.LineColor = Colors.Blue.WithAlpha(0.1 + 0.2 * i);
        }
        myPlot.Layout.Frameless();
        myPlot.Axes.Margins(0, 0);
        myPlot.Title("No sequence selected");
        return myPlot;
    }
    
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
    
    [RelayCommand]
    public async Task CheckBoxChanged(bool? isChecked)
    {
        _keepData = isChecked ?? false;
    }
    
}