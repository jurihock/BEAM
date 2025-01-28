using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using BEAM.Image;
using System.Threading;
using BEAM.ImageSequence;

namespace BEAM.Renderer;

public abstract class SequenceRenderer
{
    protected int MinimumOfIntensityRange { get; init; }
    protected int MaximumOfIntensityRange { get; init; }
    protected int IntensityRange { get; init; }

    private Dictionary<RenderTypes, SequenceRenderer> _mapRenderTypesToRenderers = new Dictionary<RenderTypes, SequenceRenderer>();

    /// <summary>
    /// Set the Minimum- and Maximum values for the intensity values (e.g. 0 - 1 or 0 - 255)
    /// This intensity is given by the user
    /// </summary>
    /// <param name="minimumOfIntensityRange"></param>
    /// <param name="maximumOfIntensityRange"></param>
    protected SequenceRenderer(int minimumOfIntensityRange, int maximumOfIntensityRange)
    {
        if (maximumOfIntensityRange <= minimumOfIntensityRange)
        {
            throw new ArgumentException("Given maximumOfIntensityRange must be greater than " +
                                        "to minimumOfIntensityRange for color intensities rendering.");
        }
        MinimumOfIntensityRange = minimumOfIntensityRange;
        MaximumOfIntensityRange = maximumOfIntensityRange;
        IntensityRange = maximumOfIntensityRange - minimumOfIntensityRange;
    }

    protected Vector256<double> NormalizeIntensity(Vector256<double> intensities)
    {
        var minIntensities = Vector256.Create<double>(MinimumOfIntensityRange);
        var maxIntensities = Vector256.Create<double>(MaximumOfIntensityRange);
        var multFactor = Vector256.Create<double>(255);

        return (intensities - minIntensities) / (maxIntensities - minIntensities) * multFactor;
    }

    public SequenceRenderer GetRenderer(RenderTypes renderType, int minimumOfIntensityRange,
        int maximumOfIntensityRange, double[] displayParameters)
    {
        SequenceRenderer renderer = _mapRenderTypesToRenderers[renderType].Create(minimumOfIntensityRange, maximumOfIntensityRange, displayParameters);

        return renderer;
    }

    /// <summary>
    /// Fill the dictionary with default renderers to map the Enums of RenderTypes to their specific classes.
    /// </summary>
    private void InitializeMapRenderTypesToRenderers()
    {
        _mapRenderTypesToRenderers.Add(RenderTypes.HeatMapRendererRb, new HeatMapRendererRB(0, 0, 0, 0, 0));
        _mapRenderTypesToRenderers.Add(RenderTypes.ChannelMapRenderer, new ChannelMapRenderer(0, 0, 0, 0, 0));
        _mapRenderTypesToRenderers.Add(RenderTypes.ArgMaxRendererGrey, new ArgMaxRendererGrey(0, 0));
    }

    public abstract byte[] RenderPixel(Sequence sequence, long x, long y);
    public abstract byte[,] RenderPixels(Sequence sequence, long[] xs, long y);

    protected abstract RenderTypes GetRenderType();

    protected abstract SequenceRenderer Create(int minimumOfIntensityRange, int maximumOfIntensityRange, double[] displayParameters);

    protected abstract bool CheckParameters(double[] displayParameters, IImage image);
}