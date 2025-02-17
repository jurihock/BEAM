using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using BEAM.Docking;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap;
using BEAM.ViewModels.Minimap.Popups;
using BEAM.Views.Minimap;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using ScottPlot;
using ShimSkiaSharp;

namespace BEAM.Image.Minimap;

/// <summary>
/// The minimap for a corresponding sequence. It creates an overview over it by calculating specific values based on an algorithm for each line.
/// Must be supplied with a concrete algorithm which is used for calculations.
/// </summary>
public class PlotMinimap : Minimap
{
    public int CompactionFactor = 100;
    /// <summary>
    /// The default minimap algorithm which will be used if this class is instantiated
    /// without a specific algorithm through its base class constructor.
    /// </summary>
    private static readonly IMinimapAlgorithm DefaultAlgorithm = new PixelSumAlgorithm();
    
    private MinimapPlotViewModel viewModel;


    /// <summary>
    /// The underlying algorithm used to calculate values for pixel lines. These values will later be displayed in the plot.
    /// </summary>
    public IMinimapAlgorithm MinimapAlgorithm;

    public PlotMinimap() : base()
    {
        MinimapAlgorithm = PlotAlgorithmSettingsUtilityHelper.GetDefaultAlgorithm();
        Console.WriteLine("minmap algo init:");
        Console.WriteLine(MinimapAlgorithm);
    }
    
    public PlotMinimap(Sequence sequence, MinimapGeneratedEventHandler eventCallbackFunc) : base(sequence, eventCallbackFunc)
    {
        CancellationTokenSource = new CancellationTokenSource();
        this.MinimapAlgorithm = DefaultAlgorithm;
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
    public PlotMinimap(Sequence sequence, MinimapGeneratedEventHandler eventCallbackFunc, IMinimapAlgorithm algorithm) : base(sequence, eventCallbackFunc)
    {
        ArgumentNullException.ThrowIfNull(algorithm);
        MinimapAlgorithm = algorithm;
        Task.Run(GenerateMinimap, CancellationTokenSource.Token);
    }

    public override void StartGeneration(Sequence sequence, MinimapGeneratedEventHandler eventCallbackFunc)
    {
        this.Sequence = sequence;
        MinimapGenerated += eventCallbackFunc;
        Console.WriteLine("Started new Minimap Generation|  " + CancellationTokenSource.ToString());
        Task.Run(GenerateMinimap, CancellationTokenSource.Token);
    }
    /// <summary>
    /// Handles the logic for creating the minimap data alongside its
    /// visual representation in the required format(<see cref="Avalonia.Controls.UserControl"/>).
    /// </summary>
    private async Task GenerateMinimap()
    {
        Console.WriteLine("Hello");
        Console.WriteLine("Generation by thread: +" + Thread.CurrentThread.ManagedThreadId + " | " + Task.CurrentId);
        //TODO: do Work with Sequence
        bool result = MinimapAlgorithm.AnalyzeSequence(Sequence, this.CancellationTokenSource.Token);
        if (!result)
        {
            // TODO: Should event also contain caller?
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Failure));
            return;
        }
        
        Plot plot = new Plot();
        Bar[] bars = new Bar[Sequence.Shape.Height / CompactionFactor];
        double maxValue = 0;
        double minValue = 0;
        for (int i = 0; i < Sequence.Shape.Height / CompactionFactor; i++)
        {
            float calculation = MinimapAlgorithm.GetLineValue(i * CompactionFactor);
            if(calculation > maxValue)
            {
                maxValue = calculation;
            } else if (calculation < minValue)
            {
                minValue = calculation;
            }
            Bar bar = new Bar();
            bar.Position = i * CompactionFactor;
            bar.Value = calculation;
            bar.Orientation = Orientation.Horizontal;
            bars[i] = bar;
            
        }
        plot.Axes.InvertY();
        plot.Add.Bars(bars);
        plot.Axes.SetLimits(left: minValue, right: maxValue, top: 0 , bottom: Sequence.Shape.Height);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            //viewModel = new MinimapPlotViewModel(plot);
            DisplayedMinimap = new MinimapPlotView() {DataContext = new MinimapPlotViewModel(plot)};
            Console.WriteLine("MinimapVM gen by: +" + Thread.CurrentThread.ManagedThreadId + " | " + Task.CurrentId);
            Console.WriteLine("Displayed minimap nullcheck innter: " + (DisplayedMinimap is null));
            Console.WriteLine("Displayed minimap nullcheck outer: " + (DisplayedMinimap is null));
            IsGenerated = true;
            Console.WriteLine("Hello World");
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Success));
        });

        /*Console.WriteLine("Displayed minimap nullcheck outer: " + (DisplayedMinimap is null));
        IsGenerated = true;
        Console.WriteLine("Hello World");
        Console.WriteLine(DisplayedMinimap.FocusAdorner.ToString());
        OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Success));*/

    }
    

    /// <summary>
    /// Returns the algorithm calculation based value for a specific line. Commonly used for plotting.
    /// </summary>
    /// <param name="line">The line whose value shall be returned.</param>
    /// <returns>The specified line's calculated value.</returns>
    /// <exception cref="InvalidOperationException">Thrown to indicate that
    /// the minimap has not yet finished its generation process.</exception>
    public float GetMinimapValue(long line)
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

    public override ISaveControl? GetSettingsPopupControl()
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
        Console.WriteLine("Cloned minimap ---------");
        Console.WriteLine("CompactionFactor: " + CompactionFactor + " | " + MinimapAlgorithm.GetName());
        return new PlotMinimap() {CompactionFactor = this.CompactionFactor, MinimapAlgorithm = this.MinimapAlgorithm};
    }

    public override string ToString()
    {
        return "Plot Minimap";
    }
}