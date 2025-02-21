using System;
using System.Threading;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.ViewModels;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap;

/// <summary>
/// An abstraction of a minimap which is based on a sequence. It displays
/// information about a sequence's contents through visual means to the user.
/// It must therefore specify its view itself.
///
/// Beware: Any concrete class implementing this class must provide a parameterless constructor due to reflection invocation.
/// </summary>
public abstract class Minimap

{
    /// <summary>
    /// The sequence based on which the minimap is based.
    /// </summary>
    protected ISequence? Sequence;
    protected bool IsGenerated { get; set; }
    /// <summary>
    /// Cancellation Token for the generation process. Any subclass should use this Token for its Threads
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; init; }
    
    /// <summary>
    /// A function blueprint for the callback function which is called concurrently/in parallel
    /// when the data generation process has finished.
    /// </summary>
    public delegate void MinimapGeneratedEventHandler(object sender, MinimapGeneratedEventArgs e);
    protected event MinimapGeneratedEventHandler? MinimapGenerated;
    
    /// <summary>
    /// A parameterless constructor. Required by every concrete class which inherits from this class for reflection invocation.
    /// </summary>
    public Minimap()
    {
        CancellationTokenSource = new();
    }

    /// <summary>
    /// Interrupts the generation process as well as all additional threads running.
    /// Useful if the corresponding sequence has been closed, yet the minimap is still running operations.
    /// </summary>
    public void StopGeneration()
    {
        CancellationTokenSource.Cancel();
    }
    
    /// <summary>
    /// Method for this class subclasses to call whenever they have
    /// finished their generation process so the original caller can be informed.
    /// </summary>
    /// <param name="e">The <see cref="MinimapGeneratedEventArgs"/> which will be used to Invoke the event.
    /// They also specify the caller.</param>
    protected  void OnMinimapGenerated(MinimapGeneratedEventArgs e)
    {
        if (MinimapGenerated is null) return;
        MinimapGenerated.Invoke(e.Minimap, e);
    }

    protected abstract String GetName();
    public abstract SaveUserControl? GetSettingsPopupControl();
    
    

    public abstract Minimap Clone();

    public String Name => GetName();
    
    public abstract void StartGeneration(ISequence sequence, MinimapGeneratedEventHandler eventCallbackFunc);

    public abstract void SetRenderer(SequenceRenderer renderer);

    public abstract ViewModelBase GetViewModel();
}