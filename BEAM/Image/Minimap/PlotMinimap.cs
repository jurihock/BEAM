using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Threading;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.ViewModels.Minimap;
using BEAM.ViewModels.Utility;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;
using ScottPlot;
using ScottPlot.Plottables;

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
    /// If a sequence has less row, use RelHeightCompactionFactor instead.
    /// </summary>
    private const int MaxHeightForRelCompaction = 15000;
    
    /// <summary>
    /// The underlying algorithm used to calculate values for pixel lines. These values will later be displayed in the plot.
    /// </summary>
    public IMinimapAlgorithm? MinimapAlgorithm;
    

    private Plot _plot = new();
    private MinimapPlotViewModel _viewModel;

    public PlotMinimap()
    {
        MinimapAlgorithm = SettingsUtilityHelper<IMinimapAlgorithm>.GetDefaultObject();
        _viewModel = new MinimapPlotViewModel(_plot, this,"Generating: " + Name);
    }

    private TransformationStorer _latestData;

    public override void StartGeneration(ISequence sequence, MinimapGeneratedEventHandler eventCallbackFunc)
    {
        this.Sequence = sequence;
        _latestData.Update(sequence);
        MinimapGenerated += eventCallbackFunc;
        Task.Run(() => GenerateMinimap(), CancellationTokenSource.Token);
    }

    public override void SetRenderer(SequenceRenderer renderer)
    {
        if(MinimapAlgorithm is null) return;
        MinimapAlgorithm.SetRenderer(renderer);
    }

    public override SizeAdjustableViewModelBase GetDisplayableViewModel()
    {
        if (!IsGenerated)
        {
            return new MinimapPlotViewModel(_plot, this);
        } 
        return _viewModel;
    }

    public override async Task CutRerender(TransformedSequence newSequence, long startCutoff, long endCutoff)
    {
        if (!IsGenerated || Sequence is null)
        {
            this.Sequence = newSequence;
            await Task.Run(() => GenerateMinimap(false), CancellationTokenSource.Token);
        }


        if (endCutoff == 0 && startCutoff == 0)
        {
            // nothing has actually changed
            return;
        }

        if (MaxHeightForRelCompaction >= newSequence.Shape.Height)
        {
            var actualCompactionUsed =
                (int)Math.Ceiling(newSequence.Shape.Height / (double)RelHeightCompactionFactor);
            this.Sequence = newSequence;
            await GeneratePlotDisregardingPrev(actualCompactionUsed);
        }
        else
        {
            CutPlotToFit(startCutoff, endCutoff);
            this.Sequence = newSequence;
        }
                
                
  
        IsGenerated = true;
        _viewModel.CurrentPlot = _plot; 
        _latestData.Update(newSequence);
    }

    public override async Task TransformationRerender(TransformedSequence newSequence)
    {
        if (!IsGenerated || Sequence is null)
        {
            this.Sequence = newSequence;
            await Task.Run(() => GenerateMinimap(false), CancellationTokenSource.Token);
        }

        IsGenerated = false;

        var isTransformed = !(Math.Abs(_latestData.ScaleX - newSequence.ScaleX) < 0.0001f
                              && Math.Abs(_latestData.ScaleY - newSequence.ScaleY) < 0.0001f
                              && Math.Abs(_latestData.OffsetY - newSequence.DrawOffsetY) < 0.0001f
                              && Math.Abs(_latestData.OffsetX - newSequence.DrawOffsetX) < 0.0001f);
        this.Sequence = newSequence;
        if (!isTransformed)
        {
            return;
        }

        if (Math.Abs(_latestData.ScaleX - newSequence.ScaleX) < 0.0001f
            && Math.Abs(_latestData.ScaleY - newSequence.ScaleY) < 0.0001f)
        {
            // Only offsets have changed, no scaling. Hence, do not recalculating the plot.
            await Task.Run(() => MovePlotByOffset(newSequence));
        }
        else
        {
            var actualCompactionUsed = CompactionFactor;
            if (MaxHeightForRelCompaction >= newSequence.Shape.Height)
            {
                actualCompactionUsed = (int)Math.Ceiling(newSequence.Shape.Height / (double)RelHeightCompactionFactor);
            }
            await Task.Run(() => GeneratePlotDisregardingPrev(actualCompactionUsed));
        }
        
        IsGenerated = true;
        _viewModel.CurrentPlot = _plot; 
        _latestData.Update(newSequence);
    }

    /// <summary>
    /// Moves a plot by the offset of the new sequence and the internally saved ones.
    /// </summary>
    private void MovePlotByOffset(TransformedSequence newSequence)
    {
        List<Bar> bars = ((BarPlot)_plot.PlottableList[0]).Bars;
        Plot newPlot = new Plot();
        List<Bar> barsToAdd = new List<Bar>();
        double maxValue = 0;
        double minValue = 0;
        foreach (var bar in bars)
        {
            if (bar.Value > maxValue)
            {
                maxValue = bar.Value;
            }
            else if (bar.Value < minValue)
            {
                minValue = bar.Value;
            }
            bar.Position += newSequence.DrawOffsetY - _latestData.OffsetY;
            barsToAdd.Add(bar);
        }
        newPlot.Add.Bars(barsToAdd.ToArray());
        _plot = newPlot;
        _plot.Axes.SetLimits(left: minValue, right: maxValue, top: 0 - ScrollBarOffset , bottom: newSequence.Shape.Height +  newSequence.DrawOffsetY + ScrollBarOffset);

    }

    /// <summary>
    /// Cuts the plot to fit a smaller sequence.
    /// </summary>
    /// <param name="startOffset">The number of lines cut from the beginning.</param>
    /// <param name="endOffset">The number of lines cut from the end.</param>
    private void CutPlotToFit(long startOffset, long endOffset)
    {
        double maxValue = 0;
        double minValue = 0;
        List<Bar> bars = ((BarPlot)_plot.PlottableList[0]).Bars;
        Plot newPlot = new Plot();
        List<Bar> barsToAdd = new List<Bar>();
        foreach (var bar in bars)
        {
            if(bar.Position >= startOffset && bar.Position < Sequence!.Shape.Height - endOffset)
            {
                bar.Position -= startOffset;
                if (bar.Value > maxValue)
                {
                    maxValue = bar.Value;
                }
                else if (bar.Value < minValue)
                {
                    minValue = bar.Value;
                }
                barsToAdd.Add(bar);
            }
        }
        newPlot.Add.Bars(barsToAdd.ToArray());
        _plot = newPlot;
        _plot.Axes.SetLimits(left: minValue, right: maxValue, top: 0 - ScrollBarOffset , bottom: Sequence!.Shape.Height - startOffset - endOffset + ScrollBarOffset + _latestData.OffsetY);
    }


    /// <summary>
    /// Handles the logic for creating the minimap data alongside its
    /// visual representation in the required format(<see cref="Avalonia.Controls.UserControl"/>).
    /// </summary>
    private async Task GenerateMinimap(bool inform = true)
    {
        using var _ = Profiling.Timer.Start("Generate Minimap");
        if (Sequence is null || MinimapAlgorithm is null)
        {
            if (inform)
            {
                OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Failure));
            }
            return;
        }
        bool result = MinimapAlgorithm.AnalyzeSequence(Sequence, this.CancellationTokenSource.Token);
        if (!result)
        {
            if (inform)
            {
                OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Failure));
            }
            return;
        }
        
        var actualCompactionUsed = CompactionFactor;
        try
        {
             await GeneratePlotDisregardingPrev(actualCompactionUsed);
        }    catch (OperationCanceledException)
        {
            //End routine, Generation process was canceled
            return;
        }
        
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _viewModel = new MinimapPlotViewModel(_plot, this);
            IsGenerated = true;
            
        });
        if (inform)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _viewModel = new MinimapPlotViewModel(_plot, this);
                IsGenerated = true;
                OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Success));
            });
        }
    }

    // This method throws an OperationCanceledException if the generation process was canceled through the CancellationToken.
    // All calling methods must furthermore ensure that MinimapAlgorithm != null.
    private async Task GeneratePlotDisregardingPrev(int compactionFactor)
    {
        _plot = new Plot();
        
        double maxValue = 0;
        double minValue = 0;
        
        if(MaxHeightForRelCompaction >= Sequence!.Shape.Height)
        {
            
            compactionFactor = (int) Math.Ceiling(Sequence.Shape.Height / (double) RelHeightCompactionFactor);
        }

        long workload = Sequence.Shape.Height / compactionFactor;
        Bar[] bars = new Bar[workload];
        
        await Dispatcher.UIThread.InvokeAsync(() => _viewModel.InitializeStatusWindow());
        for (int i = 0; i < workload; i++)
        {
            CancellationTokenSource.Token.ThrowIfCancellationRequested();
            double calculation = MinimapAlgorithm!.GetLineValue(i * compactionFactor);
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
                Position = i * compactionFactor + _latestData.OffsetY,
                Value = calculation,
                Orientation = Orientation.Horizontal
            };
            bars[i] = bar;
            _viewModel.MinimapProgress = (byte)Math.Round((i / (double)Math.Max((workload - 1), 1)) * 100);
        }
        
        await Dispatcher.UIThread.InvokeAsync(() => _viewModel.CloseStatusWindow());
        _plot.Axes.InvertY();
        _plot.Add.Bars(bars);
        _plot.Axes.SetLimits(left: minValue, right: maxValue, top: 0 - ScrollBarOffset , bottom: Sequence.Shape.Height + ScrollBarOffset + _latestData.OffsetY);

    }

    protected override void CancelGenerationVisuals()
    {
        _viewModel.CloseStatusWindow();
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