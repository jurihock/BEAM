using System;

namespace BEAM.ViewModels.Utility;

public class SizeChangedEventArgs(double newWidth, double newHeight) : EventArgs
{
    public readonly double Width = newWidth;
    public readonly double Height = newHeight;
}