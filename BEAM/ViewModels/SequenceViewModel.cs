using System;
using BEAM.Docking;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class SequenceViewModel : ViewModelBase, IDockBase
{
    public string Name { get; } = "Eine tolle Sequence";
}