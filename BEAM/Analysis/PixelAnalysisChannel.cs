using Avalonia.Threading;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using ScottPlot;


namespace BEAM.Analysis;

public class PixelAnalysisChannel : Analysis
{

    private const string Name = "Pixel Channel Analysis";

    private double[] _channels = [];

    protected override void PerformAnalysis(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint,
        ISequence sequence, InspectionViewModel inspectionViewModel)
    {
        _channels = sequence.GetPixel((long)pointerPressedPoint.Column, (long)pointerReleasedPoint.Row);
    }

    protected override Plot GetAnalysisPlot()
    {
        var plot = PlotCreator.CreateFormattedBarPlot(_channels);
        plot.Title(Name);
        return plot;
    }

    public override string ToString()
    {
        return Name;
    }
}