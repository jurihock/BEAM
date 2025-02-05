using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using ScottPlot;

namespace BEAM.Analysis;

public class HistogramAnalysis : IPixelAnalysis
{
    public Plot analyseRegion(Sequence sequence, Shape region)
    {
        throw new System.NotImplementedException();
    }

    public Plot analyseRegion(SequenceViewModel viewModel, Shape region)
    {
        throw new System.NotImplementedException();
    }

    public Plot analyseRegion(long posX, long posY, long width, long height, SequenceViewModel viewModel)
    {
        throw new System.NotImplementedException();
    }

    public Plot analysePixel(Sequence sequence, Coordinate2D position)
    {
        throw new System.NotImplementedException();
    }

    public Plot analysePixel(SequenceViewModel viewModel, Coordinate2D position)
    {
        throw new System.NotImplementedException();
    }
    
    public override string ToString()
    {
        return "Histogram Analysis";
    }
}