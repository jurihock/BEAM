using System;
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
    public partial long Offset { get; set; } = 0;

    /// <summary>
    /// The maximum possible value to cut the old sequence (height - 1).
    /// Therefore the minimum height of the new cut sequence is 1.
    /// </summary>
    [ObservableProperty] public partial long MaxOffset { get; set; }

    public CutSequencePopupViewModel(SequenceViewModel model)
    {
        _sequenceViewModel = model;
        MaxOffset = _sequenceViewModel.Sequence.Shape.Height - 1;
    }

    /// <summary>
    /// Saves the currently entered settings.
    /// </summary>
    /// <returns>Whether the sequence could be cut successfully</returns>
    public bool Save()
    {
        if (Offset < 0 || Offset > MaxOffset)
        {
            return false;
        }

        _sequenceViewModel.Sequence = new TransformedSequence(new CutSequence(_sequenceViewModel.Sequence.GetName(),
            Offset, _sequenceViewModel.Sequence));
        _sequenceViewModel.CutSequence(this, new RenderersUpdatedEventArgs(Offset));
        return true;
    }
}