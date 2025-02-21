using System;
using System.Threading;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

/// <summary>
/// An algorithm which calculates values for each pixel line in a sequence.
/// How and based on which parameters these values are being calculated are heavily depended on the concrete implementation.
///
/// Beware: Any concrete class implementing this interface must provide a parameterless constructor due to reflection invocation.
/// </summary>
public interface IMinimapAlgorithm
{
    /// <summary>
    /// A concrete implementation of an algorithms which generates values for all lines of a sequence.
    /// Assume that the method SetRenderer has been called before this method is called.
    /// </summary>
    /// <param name="sequence"> The sequence based on which the values are being calculated.</param>
    /// <param name="ctx"> The cancellation token which is used to cancel the generation process.</param>
    /// <returns> A Boolean representing whether the generation finished successfully.</returns>
    bool AnalyzeSequence(ISequence sequence, CancellationToken ctx);

    /// <summary>
    /// Returns the algorithm calculation based value for a specific line. Commonly used for plotting.
    /// </summary>
    /// <param name="line">The line whose value shall be returned.</param>
    /// <returns>The specified line's calculated value.</returns>
    double GetLineValue(long line);

    /// <summary>
    /// Gets the name of the minimap algorithm as a property.
    /// </summary>
    String GetName();


    /// <summary>
    /// Gets the name of the minimap algorithm.
    /// </summary>
    /// <returns>A string representing the algorithm's name.</returns>
    public String Name => GetName();


    /// <summary>
    /// Gets a user control for algorithm-specific settings, if any exist.
    /// </summary>
    /// <returns>A SaveUserControl instance, or null if no settings are available.</returns>
    public SaveUserControl? GetSettingsPopupControl();

    /// <summary>
    /// Creates a deep copy of the minimap algorithm instance.
    /// </summary>
    /// <returns>A new instance of the minimap algorithm.</returns>
    public IMinimapAlgorithm Clone();

    /// <summary>
    /// Sets the sequence renderer used by the algorithm for pixel rendering.
    /// Must be called before AnalyzeSequence.
    /// </summary>
    /// <param name="renderer">The sequence renderer instance to use.</param>
    public void SetRenderer(SequenceRenderer renderer);


}