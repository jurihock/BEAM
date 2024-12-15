using BEAM.ImageSequence;

namespace BEAM.Image.Minimap.Utility;

public interface MinimapAlgorithm
{
    /// <summary>
    /// A concrete implementation of an algorithms which generates values for all lines of a sequence.
    /// </summary>
    /// <param name="sequence"> THe sequence based on which the values are being calculated.</param>
    /// <returns>A float array which stores the values for each of the sequences line.</returns>
    float[] analyzeSequence(Sequence sequence);
}