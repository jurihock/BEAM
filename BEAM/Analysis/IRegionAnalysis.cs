using Avalonia;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.Views.AnalysisView;

namespace BEAM.Analysis;

/// <summary>
/// Interface for subclasses to analyse the channels a region of pixels.
/// Implemented as a Strategy design pattern.
/// </summary>
public interface IRegionAnalysis
{
    public AbstractAnalysisView analyseRegion(Sequence sequence, Shape region);
    public AbstractAnalysisView analyseRegion(SequenceViewModel viewModel, Shape region);

    public AbstractAnalysisView analyseRegion(long posX, long posY, long width, long height,
        SequenceViewModel viewModel);
}