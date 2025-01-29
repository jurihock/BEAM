using System;
using System.Threading.Tasks;
using BEAM.Docking;
using BEAM.ImageSequence;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class SequenceViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] public partial DockingViewModel DockingVm { get; set; } = new();
    
    public Sequence Sequence { get;}
    public SequenceViewModel(Sequence sequence, DockingViewModel dockingVm)
    {
        Sequence = sequence;
        DockingVm = dockingVm;
    }

    [RelayCommand]
    public async Task OpenInspectionView(SequenceViewModel sequenceViewModel)
    {
        DockingVm.OpenDock(new InspectionViewModel(sequenceViewModel));
    }
    
    public string Name { get; } = "Eine tolle Sequence";
}