using System;
using BEAM.Docking;
using BEAM.ImageSequence;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class SequenceViewModel : ViewModelBase, IDockBase
{
    public Sequence Sequence { get;}
    public SequenceViewModel(Sequence sequence)
    {
        Sequence = sequence;
    }
    
    public string Name { get; } = "Eine tolle Sequence";
}