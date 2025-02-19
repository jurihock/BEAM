using System;
using System.Threading;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

public class PixelSumAlgorithm : IMinimapAlgorithm
{
    private Sequence? _sequence;
    private CancellationToken? _ctx;
    private int[]? _channelFetchMask;


    public bool AnalyzeSequence(Sequence sequence, CancellationToken ctx)
    {
        _sequence = sequence;
        _ctx = ctx;
        _channelFetchMask = new int[sequence.Shape.Channels];

        for (int i = 0; i < sequence.Shape.Channels; i++)
        {
            _channelFetchMask[i] = i;
        }


        return true;
    }

    private float AnalyzeLine(long line)
    {
        LineImage lineExcerpt = _sequence!.GetPixelLineData(line, _channelFetchMask!);
        float sum = 0.0f;
        for (long j = 0; j < _sequence.Shape.Width; j++)
        {
            foreach (double channelValue in lineExcerpt.GetPixel(j, 0))
            {
                sum += (float)channelValue;
            }
        }

        return sum;
    }

    public float GetLineValue(long line)
    {
        if (_sequence is null || _ctx is null || _channelFetchMask is null)
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
    }
}
    