using System;
using System.Collections.Generic;
using System.Linq;
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

    public long TransOffsetY { get; set; } = 0;

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
        Task.Run(() => GenerateMinimap(), CancellationTokenSource.Token);
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

    public override async Task CutRerender(TransformedSequence newSequence, long startCutoff, long endCutoff)
    {
        Console.WriteLine("Minimap recieved request: " + newSequence.Shape.Width + " " + newSequence.Shape.Height + "|||" + startCutoff + ";;" + endCutoff);
        if (!IsGenerated || Sequence is null)
        {
            Console.WriteLine("Aborted prematurely");
            this.Sequence = newSequence;
            await Task.Run(() => GenerateMinimap(false), CancellationTokenSource.Token);
        }


        if (endCutoff == 0 && startCutoff == 0)
        {
            Console.WriteLine("no change");
            return;
        }
        else
        {
            Console.WriteLine("only cutting");
            if (MaxHeightForRelCompaction >= newSequence.Shape.Height)
            {
                Console.WriteLine("Using exact calculation");
                var actualCompactionUsed =
                    (int)Math.Ceiling(newSequence.Shape.Height / (double)RelHeightCompactionFactor);
                this.Sequence = newSequence;
                GeneratePlotDisregardingPrev(actualCompactionUsed);
            }
            else
            {
                Console.WriteLine("Using user specified calculation");
                CutPlotToFit(startCutoff, endCutoff);
                this.Sequence = newSequence;
            }
                
                
            if (_viewModel is null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _viewModel = new MinimapPlotViewModel(_plot);
                });
            } 
            IsGenerated = true;
            _viewModel!.CurrentPlot = _plot; 
            Console.WriteLine("Changed plot");
        }
    }

    public override async Task TransformationRerender(TransformedSequence newSequence)
    {
        TransOffsetY = (long) newSequence.DrawOffsetY;
        this.Sequence = newSequence;
        if (!IsGenerated || Sequence is null)
        {
            Console.WriteLine("Aborted prematurely");
            this.Sequence = newSequence;
            await Task.Run(() => GenerateMinimap(false), CancellationTokenSource.Token);
        }
        bool isTransformed = false;
        try
        {
            isTransformed = !(Math.Abs(((TransformedSequence)Sequence).ScaleX - newSequence.ScaleX) < 0.0001f
                              && Math.Abs(((TransformedSequence)Sequence).ScaleY - newSequence.ScaleY) < 0.0001f);
        }
        catch (Exception ex)
        {
            isTransformed = !(Math.Abs(newSequence.ScaleX - 1) < 0.0001f && Math.Abs(newSequence.ScaleY - 1) < 0.0001f);
        }

        if (!isTransformed)
        {
            return;
        }

        var actualCompactionUsed = CompactionFactor;
        if (MaxHeightForRelCompaction >= newSequence.Shape.Height)
        {
            Console.WriteLine("Using exact calculation");
            actualCompactionUsed = (int)Math.Ceiling(newSequence.Shape.Height / (double)RelHeightCompactionFactor);
        }

        GeneratePlotDisregardingPrev(actualCompactionUsed);
        
        if (_viewModel is null)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _viewModel = new MinimapPlotViewModel(_plot);
            });
        } 
        IsGenerated = true;
        _viewModel!.CurrentPlot = _plot; 
        
        
    }
    
    private void CutPlotToFit(long startOffset, long endOffset)
    {
        double maxValue = 0;
        double minValue = 0;
        Console.WriteLine("Cutting");
        List<Bar> bars = ((BarPlot)_plot.PlottableList[0]).Bars;
        Plot newPlot = new Plot();
        List<Bar> barsToAdd = new List<Bar>();
        foreach (var bar in bars)
        {
            Console.WriteLine("bar position: " + bar.Position);
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
                Console.WriteLine("Added bar from position " + bar.Position);
                barsToAdd.Add(bar);
            }
        }
        newPlot.Add.Bars(barsToAdd.ToArray());
        _plot = newPlot;
        _plot.Axes.SetLimits(left: minValue, right: maxValue, top: 0 - ScrollBarOffset , bottom: Sequence!.Shape.Height - startOffset - endOffset + ScrollBarOffset + TransOffsetY);
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
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Failure));
            return;
        }
        bool result = MinimapAlgorithm.AnalyzeSequence(Sequence, this.CancellationTokenSource.Token);
        if (!result)
        {
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Failure));
            return;
        }
        
        var actualCompactionUsed = CompactionFactor;
        try
        {
             GeneratePlotDisregardingPrev(actualCompactionUsed);
        }    catch (OperationCanceledException e)
        {
            //End routine, Generation process was canceled
            return;
        }
        
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _viewModel = new MinimapPlotViewModel(_plot);
            IsGenerated = true;
            
        });
        if (inform)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _viewModel = new MinimapPlotViewModel(_plot);
                IsGenerated = true;
                OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Success));
            });
        }
    }

    // This method throws an OperationCanceledException if the generation process was canceled through the CancellationToken.
    // All calling methods must furthermore ensure that MinimapAlgorithm != null.
    private void GeneratePlotDisregardingPrev(int compactionFactor)
    {
        _plot = new Plot();
      
        double maxValue = 0;
        double minValue = 0;
        
        if(MaxHeightForRelCompaction >= Sequence!.Shape.Height)
        {
            
            compactionFactor = (int) Math.Ceiling(Sequence.Shape.Height / (double) RelHeightCompactionFactor);
        }
        Bar[] bars = new Bar[Sequence.Shape.Height / compactionFactor];
        

        for (int i = 0; i < Sequence.Shape.Height / compactionFactor; i++)
        {
            CancellationTokenSource.Token.ThrowIfCancellationRequested();
            double calculation = MinimapAlgorithm.GetLineValue(i * compactionFactor);
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
                Position = i * compactionFactor + TransOffsetY,
                Value = calculation,
                Orientation = Orientation.Horizontal
            };
            bars[i] = bar;
        }
        
        _plot.Axes.InvertY();
        _plot.Add.Bars(bars);
        _plot.Axes.SetLimits(left: minValue, right: maxValue, top: 0 - ScrollBarOffset , bottom: Sequence.Shape.Height + ScrollBarOffset + TransOffsetY);

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