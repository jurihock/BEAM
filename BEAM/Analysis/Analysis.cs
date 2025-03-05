using System;
using System.Diagnostics.Tracing;
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
        try
        {
            Task.Run(() =>
            {
                inspectionViewModel.AnalysisProgress = 0;
                PerformAnalysis(pointerPressedPoint, pointerReleasedPoint, sequence, inspectionViewModel,
                    cancellationToken);
                SetPlot(inspectionViewModel);
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Logger.GetInstance().Warning(LogEvent.Analysis, "Analysis cancelled");
            Dispatcher.UIThread.Post(inspectionViewModel.AnalysisEnded);
        }
    }

    /// <summary>
    /// Performs the analysis on the sequence.
    /// </summary>
    /// <param name="pointerPressedPoint"></param>
    /// <param name="pointerReleasedPoint"></param>
    /// <param name="sequence"></param>
    /// <param name="inspectionViewModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns> A plot displaying the result of the Analysis.</returns>
    protected abstract void PerformAnalysis(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint,
        ISequence sequence, InspectionViewModel inspectionViewModel, CancellationToken cancellationToken);

    public abstract override string ToString();

    protected abstract Plot GetAnalysisPlot();

    /// <summary>
    /// Called by the asyncronous threads to update the plot in the InspectionViewModel
    /// </summary>
    /// <param name="inspectionViewModel"></param>
    private async void SetPlot(InspectionViewModel inspectionViewModel)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var plot = GetAnalysisPlot();
            inspectionViewModel.CurrentPlot = plot;
            inspectionViewModel.AnalysisEnded();
        });
    }

    protected void CheckAndCancelAnalysis(InspectionViewModel inspectionViewModel, CancellationToken token)
    {
        if (!token.IsCancellationRequested) return;
        Dispatcher.UIThread.Post(() => Models.Log.Logger.GetInstance().Warning(LogEvent.Analysis, "Analysis cancelled"));
        token.ThrowIfCancellationRequested();
    }

    protected static void SetProgress(InspectionViewModel inspectionViewModel, byte progress)
    {
        Dispatcher.UIThread.Post(() => inspectionViewModel.AnalysisProgress = progress);
    }
}