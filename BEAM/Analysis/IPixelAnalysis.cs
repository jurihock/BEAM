using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;

namespace BEAM.Analysis;

/// <summary>
/// Interface for subclasses to analyse the channels of single pixels.
/// Implemented as a Strategy design pattern.
/// </summary>
public interface IPixelAnalysis
{
    public Avalonia.Controls.Control analysePixel(Sequence sequence, Coordinate2D position);
    public Avalonia.Controls.Control analysePixel(SequenceViewModel viewModel, Coordinate2D position);
}