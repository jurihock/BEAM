using BEAM.ImageSequence;
using BEAM.Renderer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

/// <summary>
/// View model controlling the cut sequence popup.
/// </summary>
public partial class CutSequencePopupViewModel : ViewModelBase
{
    private readonly SequenceViewModel _sequenceViewModel;

    /// <summary>
    /// The position the new sequence will begin from (everything up to this position will be discarded from the original sequence)
    /// </summary>
    [ObservableProperty]
    public partial long StartOffset { get; set; } = 0;
    [ObservableProperty]
    public partial long EndOffset { get; set; } = 0;

    /// <summary>
    /// The maximum possible value to cut the old sequence (height - 1).
    /// Therefore the minimum height of the new cut sequence is 1.
    /// </summary>
    [ObservableProperty] public partial long MaxOffset { get; set; }

    public CutSequencePopupViewModel(SequenceViewModel model)
    {
        _sequenceViewModel = model;
        MaxOffset = _sequenceViewModel.Sequence.Shape.Height - 1;
        EndOffset = _sequenceViewModel.Sequence.Shape.Height - 1;
    }

    /// <summary>
    /// Saves the currently entered settings.
    /// </summary>
    /// <returns>Whether the sequence could be cut successfully</returns>
    public bool Save()
    {
        if (StartOffset < 0 || StartOffset > MaxOffset || EndOffset < 0 || EndOffset > MaxOffset)
        {
            return false;
        }

        var oldLength = _sequenceViewModel.Sequence.Shape.Height;
        _sequenceViewModel.Sequence = new TransformedSequence(new CutSequence(_sequenceViewModel.Sequence.GetName(),
                                        StartOffset, EndOffset, _sequenceViewModel.Sequence));
        _sequenceViewModel.CutSequence(this, new RenderersUpdatedEventArgs());
        _sequenceViewModel.CutMinimap(StartOffset, oldLength - EndOffset);
        return true;
    }
}