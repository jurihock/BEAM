using Avalonia.Controls;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;

namespace BEAM.Analysis;

public class PixelAnalysisChannel : IPixelAnalysis
    
{
    public Control analysePixel(Sequence sequence, Coordinate2D position)
    {
        double[] colors = sequence.GetPixel((long) position.Column, (long) position.Row);
        return null;
    }

    public Control analysePixel(SequenceViewModel viewModel, Coordinate2D position)
    {
        throw new System.NotImplementedException();
    }

    private Control _CreateGraph(double[] colors)
    {
        var graph = new Control();
        return null;
    }
}