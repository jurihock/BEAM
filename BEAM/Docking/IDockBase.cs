using System;

namespace BEAM.Docking;

/// Interface to signal a viewmodel is usable as a dock.
public interface IDockBase : IDisposable
{
    string Name { get; }
    public void OnClose();
}