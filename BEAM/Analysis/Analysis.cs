using BEAM.Datatypes;
using BEAM.ImageSequence;
using ScottPlot;

namespace BEAM.Analysis;

public abstract class Analysis
{
    public abstract Plot Analyze(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, ISequence sequence);

    public abstract override string ToString();
}