using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using BEAM.ViewModels;
using ScottPlot;

namespace BEAM.Analysis;

/// <summary>
/// Abstract base class for child classes implementing analysing methods.
/// These methods allow to analyse a rectangular segment of an ISequence instance and display the result as a Plot.
/// </summary>
public abstract class Analysis
{
    public Task CurrentTask = Task.CompletedTask;
    
    protected InspectionViewModel? BoundInspectionViewModel;
    
    /// <summary>
    /// Analyses the sequence in the rectangle encompassed by the rectangle parallel to the axes and the points
    /// pointerPressedPoint and pointerReleasedPoint at its edges. The created plot is stored in targetPlot
    /// Creates a seperate thread to perform the analysis.
    /// </summary>
    /// <param name="pointerPressedPoint"></param>
    /// <param name="pointerReleasedPoint"></param>
    /// <param name="sequence"></param>
    /// <param name="inspectionViewModel"></param>
    /// <param name="cancellationToken"></param>
    public void Analyze(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, ISequence sequence,
        InspectionViewModel inspectionViewModel, CancellationToken cancellationToken)
    {
        BoundInspectionViewModel = inspectionViewModel;
        CurrentTask = Task.Run(() =>
        {
            inspectionViewModel.AnalysisProgress = 0;
            PerformAnalysis(pointerPressedPoint, pointerReleasedPoint, sequence,
                cancellationToken);
            SetPlot(inspectionViewModel);
        }, cancellationToken);
    }

    public Plot AnalyzeForPlot(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint,
        ISequence sequence)
    {
        BoundInspectionViewModel = null;
        PerformAnalysis(pointerPressedPoint, pointerReleasedPoint, sequence, new CancellationToken());
        return GetAnalysisPlot();
    }

    /// <summary>
    /// Performs the analysis on the sequence.
    /// </summary>
    /// <param name="pointerPressedPoint"></param>
    /// <param name="pointerReleasedPoint"></param>
    /// <param name="sequence"></param>
    /// <param name="cancellationToken"></param>
    /// <returns> A plot displaying the result of the Analysis.</returns>
    protected abstract void PerformAnalysis(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint,
        ISequence sequence, CancellationToken cancellationToken);

    public abstract override string ToString();

    protected abstract Plot GetAnalysisPlot();

    /// <summary>
    /// Called by the asyncronous threads to update the plot in the InspectionViewModel
    /// </summary>
    /// <param name="inspectionViewModel"></param>
    private void SetPlot(InspectionViewModel inspectionViewModel)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var plot = GetAnalysisPlot();
            inspectionViewModel.CurrentPlot = plot;
            inspectionViewModel.AnalysisEnded();
        });
    }

    protected static void CheckAndCancelAnalysis(CancellationToken token)
    {
        if (!token.IsCancellationRequested) return;
        token.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Updates the current progress in the inspectionViewModel of the current analysis.
    /// </summary>
    /// <param name="inspectionViewModel"></param>
    /// <param name="progress"></param>
    protected void SetProgress(InspectionViewModel? inspectionViewModel, byte progress)
    {
        if (inspectionViewModel is null) return;
        inspectionViewModel.AnalysisProgress = progress;
    }
    
    public abstract AnalysisTypes GetAnalysisType();
    
    
    public static Analysis GetAnalysis(AnalysisTypes type)
    {
        return type switch
        {
            AnalysisTypes.PixelAnalysisChannel => new PixelAnalysisChannel(),
            AnalysisTypes.RegionAnalysisAverageOfChannels => new RegionAnalysisAverageOfChannels(),
            AnalysisTypes.RegionAnalysisStandardDeviationOfChannels => new RegionAnalysisStandardDeviationOfChannels(),
            _ => throw new InvalidOperationException("Undefined AnalysisType referenced.")
        };
    }

    public static List<Analysis> GetAllAnalysis()
    {
        List<Analysis> list = [];
        list.AddRange(Enum.GetValues<AnalysisTypes>().Select(GetAnalysis));
        return list;
    }
}