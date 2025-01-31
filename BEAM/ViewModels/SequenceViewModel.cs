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

    public Sequence Sequence { get; }

    private List<InspectionViewModel> _ConnectedInspectionViewModels = new();

    public SequenceViewModel(Sequence sequence, DockingViewModel dockingVm)
    {
        Sequence = sequence;
        DockingVm = dockingVm;
    }

    [RelayCommand]
    public async Task UpdateInspectionViewModel(Rectangle coordRectangle)
    {
        var topLeftX = coordRectangle.TopLeft.Column;
        var topLeftY = coordRectangle.TopLeft.Row;
        var bottomRightX = coordRectangle.BottomRight.Column;
        var bottomRightY = coordRectangle.BottomRight.Row;
        
        if (_ConnectedInspectionViewModels.Count == 0)
            return;
        if ( topLeftY > Sequence.Shape.Height || topLeftY < 0 || topLeftX > Sequence.Shape.Width ||
            topLeftX < 0)
            return;
        if (!coordRectangle.BottomRight.Equals(coordRectangle.TopLeft))
        {
            if (bottomRightY > Sequence.Shape.Height || bottomRightY < 0 || bottomRightX > Sequence.Shape.Width ||
                bottomRightX < 0)
                return;
        }
        foreach (var inspectionViewModel in _ConnectedInspectionViewModels)
        {
            inspectionViewModel.Update(coordRectangle, this);
        }
    }

    [RelayCommand]
    public async Task OpenInspectionView()
    {
        InspectionViewModel inspectionViewModel = new InspectionViewModel();
        _ConnectedInspectionViewModels.Add(inspectionViewModel);
        DockingVm.OpenDock(inspectionViewModel);
    }

    public string Name { get; } = "Eine tolle Sequence";
}