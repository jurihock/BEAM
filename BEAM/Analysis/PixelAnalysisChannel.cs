using BEAM.Datatypes;
using BEAM.ImageSequence;
using ScottPlot;


namespace BEAM.Analysis;

public class PixelAnalysisChannel : Analysis
{

    private const string Name = "Pixel Channel Analysis";

    protected override Plot PerformAnalysis(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, ISequence sequence)
    {
        double[] channels = sequence.GetPixel((long)pointerPressedPoint.Column, (long)pointerReleasedPoint.Row);

        Plot plot = PlotCreator.CreateFormattedBarPlot(channels);
        plot.Title(Name);
        return plot;
    }

    public override string ToString()
    {
        return Name;
    }
}