using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.Views.AnalysisView;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;

namespace BEAM.Analysis;

public class PixelAnalysisChannel : Analysis
{
    
    private const string Name = "Pixel Channel Analysis";
    
    public override Plot Analyze(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, Sequence sequence)
    {
        double[] channels = sequence.GetPixel((long)pointerPressedPoint.Column, (long)pointerReleasedPoint.Row);

        Plot plot = PlotCreator.createFormattedBarPlot(channels);
        plot.Title("Pixel Channel Analysis");
        return plot;
    }

    public override string ToString()
    {
        return Name;

    }
}