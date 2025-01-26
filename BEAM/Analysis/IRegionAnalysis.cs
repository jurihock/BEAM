using Avalonia;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;

namespace BEAM.Analysis;

/// <summary>
/// Interface for subclasses to analyse the channels a region of pixels.
/// Implemented as a Strategy design pattern.
/// </summary>
public interface IRegionAnalysis
{
    public Avalonia.Controls.Control analyseRegion(Sequence sequence, Shape region);
    public Avalonia.Controls.Control analyseRegion(SequenceViewModel viewModel, Shape region);

    public Avalonia.Controls.Control analyseRegion(long posX, long posY, long width, long height,
        SequenceViewModel viewModel);
}