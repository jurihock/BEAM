using System.Collections.Generic;
using System.Linq;
using BEAM.Views;
using ScottPlot.Avalonia;

namespace BEAM.ImageSequence.Synchronization;

/// <summary>
/// This class synchronizes scrolling using the ScrollBars between different sequences.
/// </summary>
public static class ScrollingSynchronizerMapper
{
    /// <summary>
    /// Used to signal, whether the ScrollBars are supposed to be synchronized.
    /// </summary>
    private static bool _isSynchronizing;
    
    private static readonly List<SequenceView> Sequences = new List<SequenceView>();
    private static readonly Dictionary<AvaPlot, SequenceView> ViewMapping = new Dictionary<AvaPlot, SequenceView>();

    /// <summary>
    /// This method is used to activate the synchronization.
    /// </summary>
    public static void ActivateSynchronization()
    {
        _isSynchronizing = true;
    }

    /// <summary>
    /// This method is used to deactivate the synchronization.
    /// </summary>
    public static void DeactivateSynchronization()
    {
        _isSynchronizing = false;
    }
    
    /// <summary>
    /// Adds a Sequence to the synchronization.
    /// </summary>
    /// <param name="sequence">The Sequence which will be added to the synchronization.</param>
    public static void AddSequence(SequenceView sequence)
    {
        Sequences.Add(sequence);
        ViewMapping.Add(sequence.AvaPlot1, sequence);
    }

    /// <summary>
    /// Retrieves the SequenceView associated with the given AvaPlot.
    /// </summary>
    /// <param name="plot">The AvaPlot instance.</param>
    /// <returns>The associated SequenceView.</returns>
    public static SequenceView GetSequenceView(AvaPlot plot)
    {
        return ViewMapping[plot];
    }
    
    /// <summary>
    /// Synchronizes the sequences to the value of a given sequence, by setting their values to that of the given sequence.
    /// </summary>
    /// <param name="sequenceView">The SequenceView containing the sequence, which values will be used for all the stored sequences.</param>
    public static void Synchronize(SequenceView sequenceView)
    {
        if (!_isSynchronizing) return;
        foreach (var view in Sequences.Where(view => view != sequenceView))
        {
            view.UpdateScrolling(sequenceView.AvaPlot1);
        }
    }

    /// <summary>
    /// Used to trigger the update for a ScrollBar linked to the Sequence of the given plot.
    /// </summary>
    /// <param name="plot">The AvaPlot, which ScrollBar will be updated.</param>
    public static void UpdateOwnScrollBar(AvaPlot plot)
    {
        ViewMapping[plot].UpdateScrollBar();
    }
}