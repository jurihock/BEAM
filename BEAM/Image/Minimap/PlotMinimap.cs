using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using BEAM.Renderer;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;
using ScottPlot;

namespace BEAM.Image.Minimap;

/// <summary>
/// The minimap for a corresponding sequence. It creates an overview over it by calculating specific values based on an algorithm for each line.
/// Has an algorithm which is used for concrete value calculation. Displays its result as a scottplot bar chart.
/// </summary>
public class PlotMinimap : Minimap
{
    
    /// <summary>
    /// The number of pixels the axis limit is offset from the first/last bar.
    /// </summary>
    private const int ScrollBarOffset = 100;
    
    /// <summary>
    /// Every x's line value will be calculated and displayed in the plot.
    /// </summary>
    public int CompactionFactor = 100;

    /// <summary>
    /// Dividend factor to divide height by to get lines per bar
    /// </summary>
    private const int RelHeightCompactionFactor = 2000;
    /// <summary>
    /// If a sequence has less row, use ReplacementCompaction instead.
    /// </summary>
    private const int MaxHeightForRelCompaction = 15000;
    
    /// <summary>
    /// The underlying algorithm used to calculate values for pixel lines. These values will later be displayed in the plot.
    /// </summary>
    public IMinimapAlgorithm? MinimapAlgorithm;

    private Plot _plot = new Plot();
    private MinimapPlotViewModel? _viewModel;

    public PlotMinimap()
    {
        MinimapAlgorithm = SettingsUtilityHelper<IMinimapAlgorithm>.GetDefaultObject();
    }
    

    public override void StartGeneration(ISequence sequence, MinimapGeneratedEventHandler eventCallbackFunc)
    {
        this.Sequence = sequence;
        MinimapGenerated += eventCallbackFunc;
        Task.Run(GenerateMinimap, CancellationTokenSource.Token);
    }

    public override void SetRenderer(SequenceRenderer renderer)
    {
        if(MinimapAlgorithm is null) return;
        MinimapAlgorithm.SetRenderer(renderer);
    }

    public override ViewModelBase GetDisplayableViewModel()
    {
        if (!IsGenerated || _viewModel is null)
        {
            return new MinimapPlotViewModel(_plot);
        } 
        return _viewModel;
    }

    /// <summary>
    /// Handles the logic for creating the minimap data alongside its
    /// visual representation in the required format(<see cref="Avalonia.Controls.UserControl"/>).
    /// </summary>
    private async Task GenerateMinimap()
    {
        using var _ = Profiling.Timer.Start("Generate Minimap");
        if (Sequence is null || MinimapAlgorithm is null)
        {
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Failure));
            return;
        }
        bool result = MinimapAlgorithm.AnalyzeSequence(Sequence, this.CancellationTokenSource.Token);
        if (!result)
        {
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Failure));
            return;
        }
        
        int actualCompactionUsed = CompactionFactor;
        _plot = new Plot();
      
        double maxValue = 0;
        double minValue = 0;
        
        if(MaxHeightForRelCompaction >= Sequence.Shape.Height)
        {
            
            actualCompactionUsed = (int) Math.Ceiling(Sequence.Shape.Height / (double) RelHeightCompactionFactor);
        }
        Bar[] bars = new Bar[Sequence.Shape.Height / actualCompactionUsed];

        try
        {
            for (int i = 0; i < Sequence.Shape.Height / actualCompactionUsed; i++)
            {
                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                double calculation = MinimapAlgorithm.GetLineValue(i * actualCompactionUsed);
                if (calculation > maxValue)
                {
                    maxValue = calculation;
                }
                else if (calculation < minValue)
                {
                    minValue = calculation;
                }

                Bar bar = new Bar
                {
                    Position = i * actualCompactionUsed,
                    Value = calculation,
                    Orientation = Orientation.Horizontal
                };
                bars[i] = bar;
            }
        }

        catch (OperationCanceledException e)
        {
            Logger.GetInstance().Info(LogEvent.BasicMessage,$"Minimap Generation was canceled: {e.Source}");
            //End routine, Generation process was canceled
            return;
        }
        _plot.Axes.InvertY();
        _plot.Add.Bars(bars);
        _plot.Axes.SetLimits(left: minValue, right: maxValue, top: 0 - ScrollBarOffset , bottom: Sequence.Shape.Height + ScrollBarOffset);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _viewModel = new MinimapPlotViewModel(_plot);
            IsGenerated = true;
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Success));
        });
    }

    protected override string GetName()
    {
        return "Plot Minimap";
    }

    public override SaveUserControl GetSettingsPopupControl()
    {
        return new PlotMinimapConfigControlView(this);
    }

    public override Minimap Clone()
    {
        return new PlotMinimap() {CompactionFactor = CompactionFactor, MinimapAlgorithm = MinimapAlgorithm};
    }

    public override string ToString()
    {
        return "Plot Minimap";
    }
}