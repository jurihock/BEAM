using Avalonia.Controls;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.Views.AnalysisView;

namespace BEAM.Analysis;

public class PixelAnalysisChannel : IPixelAnalysis
    
{
    public AbstractAnalysisView analysePixel(Sequence sequence, Coordinate2D position)
    {
        double[] colors = sequence.GetPixel((long) position.Column, (long) position.Row);
        return null;
    }

    public AbstractAnalysisView analysePixel(SequenceViewModel viewModel, Coordinate2D position)
    {
        throw new System.NotImplementedException();
    }

    private AbstractAnalysisView _CreateGraph(double[] colors)
    {
        var graph = new Control();
        return null;
    }
}