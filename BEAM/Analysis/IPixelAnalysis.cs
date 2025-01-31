using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.Views.AnalysisView;

namespace BEAM.Analysis;

/// <summary>
/// Interface for subclasses to analyse the channels of single pixels.
/// Implemented as a Strategy design pattern.
/// </summary>
public interface IPixelAnalysis
{
    public AbstractAnalysisView analysePixel(Sequence sequence, Coordinate2D position);
    public AbstractAnalysisView analysePixel(SequenceViewModel viewModel, Coordinate2D position);
}