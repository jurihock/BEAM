using System.Collections.Generic;
using Avalonia.Controls;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.Views.AnalysisView;
using ScottPlot;
using ScottPlot.Plottables;

namespace BEAM.Analysis;

public class PixelAnalysisChannel : IPixelAnalysis
{
    public Plot analysePixel(Sequence sequence, Coordinate2D position)
    {
        double[] channels = sequence.GetPixel((long)position.Column, (long)position.Row);

        Plot plot = new Plot();
        plot.Add.Bars(channels);
        return plot;
    }

    public Plot analysePixel(SequenceViewModel viewModel, Coordinate2D position)
    {
        throw new System.NotImplementedException();
    }

    public override string ToString()
    {
        return "Pixel Channel Analysis";
    }
}