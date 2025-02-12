// (c) Paul Stier, 2025

using System;

namespace BEAM.Renderer;

public class RenderersUpdatedEventArgs : EventArgs
{
    public long StartLine = 0;

    public RenderersUpdatedEventArgs(long offset)
    {
        StartLine = offset;
    }
}