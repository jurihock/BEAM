using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.ImageSequence;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScottPlot;
using Svg;

namespace BEAM.ViewModels;

public partial class SequenceViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] public partial DockingViewModel DockingVm { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D pressedPointerPosition { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D releasedPointerPosition { get; set; } = new();


    public Sequence Sequence { get; }

    private List<InspectionViewModel> _ConnectedInspectionViewModels = new();

    public SequenceViewModel(Sequence sequence, DockingViewModel dockingVm)
    {
        Sequence = sequence;
        DockingVm = dockingVm;
    }

    [RelayCommand]
    public async Task UpdateInspectionViewModel(Coordinate2D point)
    {
        Console.WriteLine("Rectangle is detected:" + pressedPointerPosition.Column + ", " + releasedPointerPosition.Column);
        
        foreach (var inspectionViewModel in _ConnectedInspectionViewModels)
        {
            inspectionViewModel.Update(point, this);
        }
    }

    [RelayCommand]
    public async Task OpenInspectionView()
    {
        InspectionViewModel inspectionViewModel = new InspectionViewModel(this);
        _ConnectedInspectionViewModels.Add(inspectionViewModel);
        DockingVm.OpenDock(inspectionViewModel);
        
        inspectionViewModel.Update(pressedPointerPosition, this);
    }

    public string Name { get; } = "Eine tolle Sequence";

    public override string ToString()
    {
        return Name;
    }
}