using System;
using System.Threading;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

/// <summary>
/// An algorithm for <see cref="PlotMinimap"/>'s which creates the sum over the
/// rendered image's and divides it through the number of pixels per line.
/// </summary>
public class PixelSumAlgorithm : IMinimapAlgorithm
{
    private ISequence? _sequence;
    private CancellationToken? _ctx;
    private SequenceRenderer? _renderer;
    private long _pixelsPerLine = 1;


    public bool AnalyzeSequence(ISequence sequence, CancellationToken ctx)
    {
        _sequence = sequence;
        _ctx = ctx;
        _pixelsPerLine = sequence.Shape.Width;
        
        return true;
    }

    private double AnalyzeLine(long line)
    {
        double sum = 0.0d;
        for (long j = 0; j < _sequence!.Shape.Width; j++)
        {
            var renderedData = _renderer!.RenderPixel(_sequence, j, line);
            foreach (byte channelValue in renderedData)
            {
                sum += channelValue;
            }
        }
        return sum/_pixelsPerLine;
    }

    public double GetLineValue(long line)
    {
        if (_sequence is null || _ctx is null || _renderer is null)
        {
            throw new InvalidStateException("Data must first be initialized!");
        }

        if (line < 0 || line >= _sequence.Shape.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(line));
        }
        return AnalyzeLine(line);
    }

    public string GetName()
    {
        return "Pixel Sum";
    }

    public SaveUserControl? GetSettingsPopupControl()
    {
        return null;
    }


    public IMinimapAlgorithm Clone()
    {
        return new PixelSumAlgorithm();
    }

    public void SetRenderer(SequenceRenderer renderer)
    {
        _renderer = renderer;
    }
}
    