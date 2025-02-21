using System;

namespace BEAM.Docking;

/// <summary>Interface to signal a view model is usable as a dock.</summary>
public interface IDockBase : IDisposable
{
    /// <summary>
    /// The name of the plot. Used to display a title to the user.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Function to handle the closing of the dock. Called when the user chooses to close it.
    /// </summary>
    public void OnClose();
}