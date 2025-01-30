using System;
using System.Threading.Tasks;
using Avalonia;
using BEAM.Docking;
using BEAM.ImageSequence;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Svg;

namespace BEAM.ViewModels;

public partial class SequenceViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] public partial DockingViewModel DockingVm { get; set; } = new();
    
    public Sequence Sequence { get;}
    
    private InspectionViewModel _CurrentInspectionViewModel = null;
    public SequenceViewModel(Sequence sequence, DockingViewModel dockingVm)
    {
        Sequence = sequence;
        DockingVm = dockingVm;
    }
    
    [RelayCommand]
    public async Task UpdateInspectionViewModel((long X, long Y) posSeq)
    {
        if (_CurrentInspectionViewModel == null)
            return;
        if (posSeq.Y > Sequence.Shape.Height || posSeq.Y < 0 || posSeq.X > Sequence.Shape.Width || posSeq.X < 0)
            return;
        double[] pixelData = Sequence.GetPixel(posSeq.X, posSeq.Y);
        _CurrentInspectionViewModel.UpdatePixelData(pixelData);
    }

    [RelayCommand]
    public async Task OpenInspectionView(SequenceViewModel sequenceViewModel)
    {
        _CurrentInspectionViewModel = new InspectionViewModel(sequenceViewModel);
        DockingVm.OpenDock(_CurrentInspectionViewModel);
    }
    
    public string Name { get; } = "Eine tolle Sequence";
}