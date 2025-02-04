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
    public IPlottable analysePixel(Sequence sequence, Coordinate2D position)
    {
        double[] colors = sequence.GetPixel((long)position.Column, (long)position.Row);
        List<Bar> barList = new List<Bar>();
        foreach (var color in colors)
        {
            var bar = new Bar();
            bar.Value = color;
            barList.Add(bar);
        }
        IPlottable barPlot = new BarPlot(barList);
        return barPlot;
    }

    public IPlottable analysePixel(SequenceViewModel viewModel, Coordinate2D position)
    {
        throw new System.NotImplementedException();
    }
}