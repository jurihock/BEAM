using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using BEAM.Datatypes.Color;
using BEAM.ImageSequence;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Renderer;

public abstract partial class SequenceRenderer : ObservableObject, ICloneable
{
    [ObservableProperty] private double _minimumOfIntensityRange;
    [ObservableProperty] private double _maximumOfIntensityRange;

    protected double IntensityRange => MaximumOfIntensityRange - MinimumOfIntensityRange;

    private readonly Dictionary<RenderTypes, SequenceRenderer> _mapRenderTypesToRenderers = new();

    // variables used for NormalizeIntensity
    private Vector256<double> _minIntensities;
    private Vector256<double> _maxIntensities;
    private Vector256<double> _multFactor;

    /// <summary>
    /// Set the Minimum- and Maximum values for the intensity values (e.g. 0 - 1 or 0 - 255)
    /// This intensity is given by the user
    /// </summary>
    /// <param name="minimumOfIntensityRange"></param>
    /// <param name="maximumOfIntensityRange"></param>
    protected SequenceRenderer(double minimumOfIntensityRange, double maximumOfIntensityRange)
    {
        if (maximumOfIntensityRange <= minimumOfIntensityRange)
        {
            throw new ArgumentException("Given maximumOfIntensityRange must be greater than " +
                                        "to minimumOfIntensityRange for color intensities rendering.");
        }

        MinimumOfIntensityRange = minimumOfIntensityRange;
        MaximumOfIntensityRange = maximumOfIntensityRange;

        _multFactor = Vector256.Create<double>(255);

        PropertyChanged += (_, _) =>
        {
            _minIntensities = Vector256.Create<double>(MinimumOfIntensityRange);
            _maxIntensities = Vector256.Create<double>(MaximumOfIntensityRange);
        };
    }

    protected Vector256<double> NormalizeIntensity(Vector256<double> intensities)
    {
        return (intensities - _minIntensities) / (_maxIntensities - _minIntensities) * _multFactor;
    }

    public SequenceRenderer GetRenderer(RenderTypes renderType, int minimumOfIntensityRange,
        int maximumOfIntensityRange, double[] displayParameters)
    {
        SequenceRenderer renderer = _mapRenderTypesToRenderers[renderType]
            .Create(minimumOfIntensityRange, maximumOfIntensityRange, displayParameters);

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

    public abstract BGR RenderPixel(ISequence sequence, long x, long y);

    /// <summary>
    /// Renders requested parts of a line.
    /// </summary>
    /// <param name="sequence">The sequence to render from</param>
    /// <param name="xs">The x coords use render</param>
    /// <param name="y">the line</param>
    /// <param name="bgrs">a pre-allocated array for the return values. Length = xs.Length</param>
    /// <param name="pool">A memory pool to use</param>
    /// <returns>The rendererd data</returns>
    public abstract BGR[] RenderPixels(ISequence sequence, long[] xs, long y, BGR[] bgrs);

    public abstract RenderTypes GetRenderType();

    protected abstract SequenceRenderer Create(int minimumOfIntensityRange, int maximumOfIntensityRange,
        double[] displayParameters);

    protected abstract bool CheckParameters(double[] displayParameters);

    public abstract string GetName();
    public abstract object Clone();
}