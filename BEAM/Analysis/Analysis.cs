using BEAM.Datatypes;
using BEAM.ImageSequence;
using ScottPlot;

namespace BEAM.Analysis;

/// <summary>
/// Abstract base class for child classes implementing analysing methods.
/// These methods allow to analyse a rectangular segment of an ISequence instance and display the result as a Plot.
/// </summary>
public abstract class Analysis
{
    /// <summary>
    /// Analyses the sequence in the rectangle encompassed by the rectangle parallel to the axes and the points
    /// pointerPressedPoint and pointerReleasedPoint at its edges.
    /// </summary>
    /// <param name="pointerPressedPoint"></param>
    /// <param name="pointerReleasedPoint"></param>
    /// <param name="sequence"></param>
    /// <returns> A plot displaying the result of the Analysis.</returns>
    public abstract Plot Analyze(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, ISequence sequence);

    public abstract override string ToString();
}