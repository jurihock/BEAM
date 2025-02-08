using System;
using BEAM.ImageSequence;
using BEAM.Renderer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

public partial class CutSequencePopupViewModel : ViewModelBase
{
    private readonly SequenceViewModel _sequenceViewModel;
    [ObservableProperty] public long offset = 0;
    [ObservableProperty] public long maxOffset = 0;

    public CutSequencePopupViewModel(SequenceViewModel model)
    {
        _sequenceViewModel = model;
        maxOffset = _sequenceViewModel.Sequence.Shape.Height - 1;
    }
    
    public bool Save()
    {
        _sequenceViewModel.Sequence = new TransformedSequence(new CutSequence(_sequenceViewModel.Sequence.GetName(),
                                        Offset, _sequenceViewModel.Sequence));
        _sequenceViewModel.CutSequence(this, new RenderersUpdatedEventArgs(Offset));
        return true;
    }
}