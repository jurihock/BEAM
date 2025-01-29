using BEAM.Docking;
using CommunityToolkit.Mvvm.ComponentModel;


namespace BEAM.ViewModels;

public class InspectionViewModel : ViewModelBase, IDockBase
{ 
    private SequenceViewModel CurrentSequence { get; set; }
    
    public string Name { get; } = "Inspect";
    
    public InspectionViewModel(SequenceViewModel startSequence)
    {
        CurrentSequence = startSequence;
    }
    
}