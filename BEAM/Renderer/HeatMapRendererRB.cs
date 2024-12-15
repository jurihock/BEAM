using System;
using System.IO;
using Avalonia.Media;
using BEAM.Exceptions;

namespace BEAM.Renderer;

/// <summary>
/// Concrete Renderer for a HeatMap.
/// The intensity of this channel is encoded in an ARGB-value and returned.
/// The hottest color is red, the coldest blue, always no transparency.
/// </summary>
public class HeatMapRendererRB : HeatMapRenderer
{
    protected override byte[] GetColor(double value, double min, double max)
    {
        if (value > max) // intensity above maximum --> hottest color displayed
        {
            byte[] hot = [255, 255, 0, 0]; // Color Red
            return hot;
        }
        else if (value < min) // intensity below minimum --> coldest color displayed
        {
            byte[] cold = [255, 0, 0, 255];
            return cold;
        }
        else
        {
            double range = (max - min);
            double
                relative = (value - min) /
                           range; // calculate the relative intensity inside the range between min and max
            byte intensity = (byte)Math.Floor(relative * (double)255);
            byte[] color = [255, intensity, 0, (byte)(255 - intensity)];
            return color;
        }
    }
}