using System;
using System.Threading;
using Avalonia.Controls;
using BEAM.ImageSequence;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

public class PixelSumAlgorithm : IMinimapAlgorithm
{
    private int _compactionFactor;
    private float[] _data;
    public PixelSumAlgorithm(int compactionFactor)
    {
        _compactionFactor = compactionFactor;
    }

    public bool AnalyzeSequence(Sequence sequence, CancellationToken ctx)
    {
        _data = new float[(sequence.Shape.Height / _compactionFactor) + 1];
        int[] channelFetchMask = new int[sequence.Shape.Channels];
        float sum;
        for(int i  = 0; i < sequence.Shape.Channels; i++)
        {
            channelFetchMask[i] = i;
        }
        for(long i = 0; i < sequence.Shape.Height; i += _compactionFactor)
        {
            ctx.ThrowIfCancellationRequested();
            Console.WriteLine("Line: "+ i);
            LineImage lineExcerpt = sequence.GetPixelLineData(i, channelFetchMask);
            sum = 0.0f;
            for(long j = 0; j < sequence.Shape.Width; j++)
            {
                foreach(double channelValue in lineExcerpt.GetPixel(j, 0)) {
                    sum += (float)channelValue;
                }
            }
            _data[i / _compactionFactor] = sum;
        }
        return true;
    }

    public float GetLineValue(long line)
    {
        return _data[line / _compactionFactor];
    }

    public string GetName()
    {
        return "Pixel Sum";
    }

    public (Control, ISaveControl)? GetSettingsPopupControl()
    {
        return null;
    }

    public PixelSumAlgorithm()
    {
        
    }
}