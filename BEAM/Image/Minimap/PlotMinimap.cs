using System;
using System.Threading;
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
/// Must be supplied with a concrete algorithm which is used for calculations.
/// </summary>
public class PlotMinimap : Minimap
{
    public int CompactionFactor = 100;

    /// <summary>
    /// The underlying algorithm used to calculate values for pixel lines. These values will later be displayed in the plot.
    /// </summary>
    public IMinimapAlgorithm MinimapAlgorithm;

    private Plot _plot = new Plot();
    private MinimapPlotViewModel? _viewModel;

    public PlotMinimap()
    {
        //MinimapAlgorithm = PlotAlgorithmSettingsUtilityHelper.GetDefaultAlgorithm();
        MinimapAlgorithm = SettingsUtilityHelper<IMinimapAlgorithm>.GetDefaultObject();

    }
    
    public PlotMinimap(ISequence sequence, MinimapGeneratedEventHandler eventCallbackFunc) : base(sequence, eventCallbackFunc)
    {
        CancellationTokenSource = new CancellationTokenSource();
        //this.MinimapAlgorithm = PlotAlgorithmSettingsUtilityHelper.GetDefaultAlgorithm();
        MinimapAlgorithm = SettingsUtilityHelper<IMinimapAlgorithm>.GetDefaultObject();
        Task.Run(GenerateMinimap, CancellationTokenSource.Token);
    }
    
    
    /// <summary>
    /// Initializes the minimap creation process. It creates a separately running Task which generates the values.
    /// Therefor, the minimap is not instantly ready after this method call ends hence a method which is used as a callback must be supplied.
    /// </summary>
    /// <param name="sequence">The sequence based on which the minimap is based.</param>
    /// <param name="eventCallbackFunc">A function which is invoked once the minimap has finished generating its values.
    /// This is being done through the <see cref="Minimap"/>'s MinimapGeneratedEventHandler event.</param>
    /// <param name="algorithm">The concrete algorithm used for value calculation.</param>
    /// <exception cref="ArgumentNullException">If any of the parameters is null.</exception>
    public PlotMinimap(ISequence sequence, MinimapGeneratedEventHandler eventCallbackFunc, IMinimapAlgorithm algorithm) : base(sequence, eventCallbackFunc)
    {
        ArgumentNullException.ThrowIfNull(algorithm);
        MinimapAlgorithm = algorithm;
        Task.Run(GenerateMinimap, CancellationTokenSource.Token);
    }

    public override void StartGeneration(ISequence sequence, MinimapGeneratedEventHandler eventCallbackFunc)
    {
        this.Sequence = sequence;
        MinimapGenerated += eventCallbackFunc;
        Task.Run(GenerateMinimap, CancellationTokenSource.Token);
    }

    public override void SetRenderer(SequenceRenderer renderer)
    {
        MinimapAlgorithm.SetRenderer(renderer);
    }

    public override ViewModelBase GetViewModel()
    {
        if (_viewModel is null)
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
        if (Sequence is null)
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
        
        _plot = new Plot();
        Bar[] bars = new Bar[Sequence.Shape.Height / CompactionFactor];
        double maxValue = 0;
        double minValue = 0;
        for (int i = 0; i < Sequence.Shape.Height / CompactionFactor; i++)
        {
            double calculation = MinimapAlgorithm.GetLineValue(i * CompactionFactor);
            if(calculation > maxValue)
            {
                maxValue = calculation;
            } else if (calculation < minValue)
            {
                minValue = calculation;
            }
            Bar bar = new Bar
            {
                Position = i * CompactionFactor,
                Value =  calculation,
                Orientation = Orientation.Horizontal
            };
            bars[i] = bar;
            
        }
        _plot.Axes.InvertY();
        _plot.Add.Bars(bars);
        _plot.Axes.SetLimits(left: minValue, right: maxValue, top: 0 , bottom: Sequence.Shape.Height);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _viewModel = new MinimapPlotViewModel(_plot);
            IsGenerated = true;
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Success));
        });



    }
    

    /// <summary>
    /// Returns the algorithm calculation based value for a specific line. Commonly used for plotting.
    /// </summary>
    /// <param name="line">The line whose value shall be returned.</param>
    /// <returns>The specified line's calculated value.</returns>
    /// <exception cref="InvalidOperationException">Thrown to indicate that
    /// the minimap has not yet finished its generation process.</exception>
    public double GetMinimapValue(long line)
    {
        if (!IsGenerated)
        {
            throw new InvalidOperationException();
        }

        return MinimapAlgorithm.GetLineValue(line);
    }


    public override string GetName()
    {
        return "Plot Minimap";
    }

    public override SaveUserControl GetSettingsPopupControl()
    {
        return new PlotMinimapConfigControlView(this);
    }
    


    public void SetCompactionFactor(int newFactor)
    {
        if (newFactor < 1) return;
         CompactionFactor = newFactor;
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