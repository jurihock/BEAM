using BEAM.ImageSequence;
using BEAM.Renderer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

public partial class CutSequencePopupViewModel(SequenceViewModel model) : ViewModelBase
{
    private SequenceViewModel _sequenceViewModel = model;
    [ObservableProperty] public long offset = 0;

    public bool Save()
    {
        _sequenceViewModel.Sequence = new TransformedSequence(new CutSequence(_sequenceViewModel.Sequence.GetName(),
                                        Offset, _sequenceViewModel.Sequence));
        _sequenceViewModel.CutSequence(this, new RenderersUpdatedEventArgs(Offset));
        return true;
    }
}