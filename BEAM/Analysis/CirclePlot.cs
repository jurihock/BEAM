using BEAM.Datatypes;
using BEAM.ImageSequence;
using ScottPlot;

namespace BEAM.Analysis;

public class CirclePlot : Analysis
{
    public override Plot Analyze(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, ISequence sequence)
    {
        Plot plot = new();
        plot.Add.Circle(0,0,300);
        return plot;
    }

    public override string ToString()
    {
        return nameof(CirclePlot);
    }
}