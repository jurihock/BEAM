using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using ScottPlot;

namespace BEAM.Analysis;

public class CirclePlot : Analysis
{
    public override Plot Analyze(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, Sequence sequence)
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