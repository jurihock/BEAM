using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.Views.AnalysisView;
using ScottPlot;

namespace BEAM.Analysis;

/// <summary>
/// Interface for subclasses to analyse the channels of single pixels.
/// Implemented as a Strategy design pattern.
/// </summary>
public interface IPixelAnalysis
{
    public IPlottable analysePixel(Sequence sequence, Coordinate2D position);
    public IPlottable analysePixel(SequenceViewModel viewModel, Coordinate2D position);
}