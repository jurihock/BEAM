using Avalonia;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.Views.AnalysisView;
using ScottPlot;

namespace BEAM.Analysis;

/// <summary>
/// Interface for subclasses to analyse the channels a region of pixels.
/// Implemented as a Strategy design pattern.
/// </summary>
public interface IRegionAnalysis
{
    public Plot analyseRegion(ISequence sequence, Shape region);
    public Plot analyseRegion(SequenceViewModel viewModel, Shape region);

    public Plot analyseRegion(long posX, long posY, long width, long height,
        SequenceViewModel viewModel);
}