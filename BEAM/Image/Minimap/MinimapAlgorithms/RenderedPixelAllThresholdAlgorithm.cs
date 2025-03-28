﻿using System;
using System.Threading;
using BEAM.Datatypes.Color;
using BEAM.Exceptions;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

/// <summary>
/// An algorithm for <see cref="PlotMinimap"/>'s which counts the number of rendered pixels within a line which
/// are greater or equal in value than a defined base pixel in every channel.
/// </summary>
public class RenderedPixelAllThresholdAlgorithm : IMinimapAlgorithm
{

    public byte ThresholdRed { get; set; } = 25;
    public byte ThresholdBlue { get; set; } = 25;
    public byte ThresholdGreen { get; set; } = 25;
    public byte ThresholdAlpha { get; set; } = 255;
    private ISequence? _sequence;
    private CancellationToken? _ctx;
    private SequenceRenderer? _renderer;
    private BGR _thresholds;
    private long[]? _fetchMask;

    public bool AnalyzeSequence(ISequence sequence, CancellationToken ctx)
    {
        _thresholds = new BGR(ThresholdBlue, ThresholdGreen, ThresholdRed);
        _sequence = sequence;
        _ctx = ctx;
        _fetchMask = new long[sequence.Shape.Width];
        for (int i = 0; i <_fetchMask.Length; i++)
        {
            _fetchMask[i] = i;
        }
        return true;
    }
    


    public double GetLineValue(long line)
    {
        if (_sequence is null || _ctx is null || _fetchMask is null)
        {
            throw new InvalidStateException("Data must first be initialized!");
        }
        if (line < 0 || line >= _sequence.Shape.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(line));
        }

        return AnalyzeLine(line);
    }

    private double AnalyzeLine(long line)
    {
        double sum = 0.0f;
        var bgrs = new BGR[_fetchMask!.Length];
        var renderedData = _renderer!.RenderPixels(_sequence!, _fetchMask!, line, bgrs);
        foreach (var entry in renderedData)
        {
            _ctx!.Value.ThrowIfCancellationRequested();
            if (entry.EntrywiseAllGreaterEqual(_thresholds))
            {
                sum += 1;
            }
        }
        
        return sum;
    }
    
    public string GetName()
    {
        return "Pixel All Threshold Algorithm";
    }

    public SaveUserControl GetSettingsPopupControl()
    {
        return new PixelThresholdAllSumAlgorithmConfigControlView(this);
    }


    public IMinimapAlgorithm Clone()
    {
        return new RenderedPixelAllThresholdAlgorithm { _renderer = _renderer, ThresholdRed = ThresholdRed, ThresholdGreen = ThresholdGreen, ThresholdBlue = ThresholdBlue, ThresholdAlpha = ThresholdAlpha };
    }

    public void SetRenderer(SequenceRenderer renderer)
    {
        _renderer = renderer;
    }

}