using System;
using System.Threading;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

public class PixelSumAlgorithm : IMinimapAlgorithm
{
    private ISequence? _sequence;
    private CancellationToken? _ctx;
    private SequenceRenderer? _renderer;


    public bool AnalyzeSequence(ISequence sequence, CancellationToken ctx)
    {
        _sequence = sequence;
        _ctx = ctx;
        
        return true;
    }

    private double AnalyzeLine(long line)
    {
        double sum = 0.0f;
        for (long j = 0; j < _sequence!.Shape.Width; j++)
        {
            var renderedData = _renderer!.RenderPixel(_sequence, j, line);
            foreach (byte channelValue in renderedData)
            {
                sum += channelValue;
            }
        }
        return sum;
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

    public ISaveControl? GetSettingsPopupControl()
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
    