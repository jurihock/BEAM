using System;
using System.Threading;
using BEAM.Datatypes;
using BEAM.Datatypes.Color;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using Timer = BEAM.Profiling.Timer;

namespace BEAM.Renderer;

/// <summary>
/// A Renderer using the ArgMax function. Calculates the channel with the highest intensity and encodes it's number as a
/// color.
/// </summary>
/// <param name="minimumOfIntensityRange"></param>
/// <param name="maximumOfIntensityRange"></param>
public abstract class ArgMaxRenderer(double minimumOfIntensityRange, double maximumOfIntensityRange)
    : SequenceRenderer(minimumOfIntensityRange, maximumOfIntensityRange)
{
    public override BGR RenderPixel(ISequence sequence, long x, long y)
    {
        var channels = sequence.GetPixel(x, y);
        var argMaxChannel = channels.ArgMax();
        var color = GetColor(argMaxChannel, channels.Length);

        return color;
    }

    public override BGR[] RenderPixels(ISequence sequence, long[] xs, long y)
    {
        var channels = new int[sequence.Shape.Channels];
        for (var i = 0; i < sequence.Shape.Channels; i++)
        {
            channels[i] = i;
        }

        var line = sequence.GetPixelLineData(xs, y, channels);
        var data = new BGR[xs.Length];

        for (var x = 0; x < xs.Length; x++)
        {
            var argMax = line.GetPixel(x, 0).ArgMax();

            var color = GetColor(argMax, channels.Length);

            data[x] = color;
        }

        return data;
    }

    protected abstract BGR GetColor(int channelNumber, int amountChannels);

    public override string GetName() => "ArgMax";
}