using System;
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
    private static readonly IMinimapAlgorithm DefaultAlgorithm = new PixelSumAlgorithm(100);
    
    private MinimapPlotViewModel viewModel;


    /// <summary>
    /// The underlying algorithm used to calculate values for pixel lines. These values will later be displayed in the plot.
    /// </summary>
    public IMinimapAlgorithm MinimapAlgorithm;

    public PlotMinimap()
    {
        
    }
    
    public PlotMinimap(Sequence sequence, MinimapGeneratedEventHandler eventCallbackFunc) : base(sequence, eventCallbackFunc)
    {
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
        Task.Run(GenerateMinimap, CancellationTokenSource.Token);
    }
    /// <summary>
    /// Handles the logic for creating the minimap data alongside its
    /// visual representation in the required format(<see cref="Avalonia.Controls.UserControl"/>).
    /// </summary>
    private void GenerateMinimap()
    {
        
        //TODO: do Work with Sequence
        bool result = MinimapAlgorithm.AnalyzeSequence(Sequence, this.CancellationTokenSource.Token);
        Console.WriteLine("Returned " + result);
        if (!result)
        {
            Console.WriteLine("Returend false");
            // TODO: Should event also contain caller?
            OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Failure));
            return;
        }

        Plot plot = new Plot();
        double[]values = new double[Sequence.Shape.Height / 100];
        for (int i = 0; i < Sequence.Shape.Height / 100; i++)
        {
            values[i] = MinimapAlgorithm.GetLineValue(i * 100);
            Console.WriteLine("Line " + i + " Value: " + values[i]);
        }

        plot.Add.Bars(values);
        Dispatcher.UIThread.Invoke(() =>
        {
            viewModel = new MinimapPlotViewModel(plot);
            DisplayedMinimap = new MinimapPlotView{DataContext = viewModel};
        });
        IsGenerated = true;
        Console.WriteLine("Hello World");
        OnMinimapGenerated(new MinimapGeneratedEventArgs(this, MinimapGenerationResult.Success));
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

    public override (Control, ISaveControl)? GetSettingsPopupControl()
    {
        var toReturn = new PlotMinimapConfigControlView(this);
        return (toReturn, toReturn);
    }

    public override IDockBase GetDock()
    {
        return viewModel;
    }
    
    
    public void SetCompactionFactor(int newFactor)
    {
        if (newFactor < 1) return;
         CompactionFactor = newFactor;
    }

    public override ViewModelBase GetViewModel()
    {
        throw new NotImplementedException();
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