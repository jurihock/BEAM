using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.ImageSequence;
using BEAM.Views;
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
    
    public void RegisterInspectionViewModel(InspectionViewModel inspectionViewModel)
    {
        _ConnectedInspectionViewModels.Add(inspectionViewModel);
    }
    
    public void UnregisterInspectionViewModel(InspectionViewModel inspectionViewModel)
    {
        _ConnectedInspectionViewModels.Remove(inspectionViewModel);
    }

    [RelayCommand]
    public async Task UpdateInspectionViewModel(Coordinate2D point)
    {
        Console.WriteLine("Rectangle is detected:" + pressedPointerPosition.ToString() + ", " + releasedPointerPosition.ToString());

        if (!_arePointsValid(pressedPointerPosition, releasedPointerPosition))
        {
            Console.WriteLine("Rectangle is not valid");
            return;
        }
        
        foreach (var inspectionViewModel in _ConnectedInspectionViewModels)
        {
            inspectionViewModel.Update(pressedPointerPosition, releasedPointerPosition);
        }
    }

    [RelayCommand]
    public async Task OpenInspectionView()
    {
        if(!_arePointsValid(pressedPointerPosition, releasedPointerPosition))
            return;
        InspectionViewModel inspectionViewModel = new InspectionViewModel(this);
        _ConnectedInspectionViewModels.Add(inspectionViewModel);
        DockingVm.OpenDock(inspectionViewModel);
        Console.WriteLine("Everythig is safe!");
        inspectionViewModel.Update(pressedPointerPosition, releasedPointerPosition);
    }
    
    private bool _arePointsValid(Coordinate2D point1, Coordinate2D point2)
    {
        if(point1.Column < 0 || point1.Row < 0 || point2.Column < 0 || point2.Row < 0)
            return false;
        if(point1.Column > Sequence.Shape.Width 
           || point1.Row > Sequence.Shape.Height 
           || point2.Column > Sequence.Shape.Width 
           || point2.Row > Sequence.Shape.Height)
            return false;
        return true;
    }
    
    

    public string Name { get; } = "Eine tolle Sequence";

    public override string ToString()
    {
        return Name;
    }
}