using BEAM.ImageSequence;

namespace BEAM.Image.Minimap.Utility;

public interface MinimapAlgorithm
{
    float[] analyzeSequence(Sequence sequence);
}