using BEAM.ImageSequence;
using BEAM.Renderer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

public partial class CutSequencePopupViewModel : ViewModelBase
{
    private readonly SequenceViewModel _sequenceViewModel;
    [ObservableProperty]
    public partial long StartOffset { get; set; } = 0;
    [ObservableProperty]
    public partial long EndOffset { get; set; } = 0;

    [ObservableProperty]
    public partial long MaxOffset { get; set; }

    public CutSequencePopupViewModel(SequenceViewModel model)
    {
        _sequenceViewModel = model;
        MaxOffset = _sequenceViewModel.Sequence.Shape.Height - 1;
        EndOffset = _sequenceViewModel.Sequence.Shape.Height - 1;
    }
    
    public bool Save()
    {
        if (StartOffset < 0 || StartOffset > MaxOffset || EndOffset < 0 || EndOffset > MaxOffset)
        {
            return false;
        }   
        _sequenceViewModel.Sequence = new TransformedSequence(new CutSequence(_sequenceViewModel.Sequence.GetName(),
                                        StartOffset, EndOffset, _sequenceViewModel.Sequence));
        _sequenceViewModel.CutSequence(this, new RenderersUpdatedEventArgs());
        return true;
    }
}