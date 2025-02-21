using System.Threading.Tasks;
using Avalonia.Threading;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
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
    /// If a sequence has less row, use ReplacementCompaction instead.
    /// </summary>
    private const int MinSequenceHeightForFullCompaction = 2000;

    /// <summary>
    /// The compaction used if sequences are smaller than MinSequenceHeightForFullCompaction.
    /// </summary>
    private const int ReplacementCompaction = 5;
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

    public override ViewModelBase GetViewModel()
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
        Bar[] bars = new Bar[Sequence.Shape.Height / CompactionFactor];
        double maxValue = 0;
        double minValue = 0;
        
        if(MinSequenceHeightForFullCompaction < Sequence.Shape.Height)
        {
            actualCompactionUsed = ReplacementCompaction;
        }
        
        for (int i = 0; i < Sequence.Shape.Height / actualCompactionUsed; i++)
        {
            double calculation = MinimapAlgorithm.GetLineValue(i * actualCompactionUsed);
            if(calculation > maxValue)
            {
                maxValue = calculation;
            } else if (calculation < minValue)
            {
                minValue = calculation;
            }
            Bar bar = new Bar
            {
                Position = i * actualCompactionUsed,
                Value =  calculation,
                Orientation = Orientation.Horizontal
            };
            bars[i] = bar;
            
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
        return new PlotMinimap() {CompactionFactor = this.CompactionFactor, MinimapAlgorithm = this.MinimapAlgorithm};
    }

    public override string ToString()
    {
        return "Plot Minimap";
    }
}