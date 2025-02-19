using System;
using System.Threading;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

public class RenderedPixelThresholdAlgorithm : IMinimapAlgorithm
{

    public byte ThresholdRed { get; set; } = 25;
    public byte ThresholdBlue { get; set; } = 25;
    public byte ThresholdGreen { get; set; } = 25;
    public byte ThresholdGamma {get; set;} = 255 ;
    private Sequence? _sequence;
    private CancellationToken? _ctx;
    private SequenceRenderer? _renderer;
    Vector4D<byte> _thresholds = new Vector4D<byte>(0, 0, 0, 255);
    public bool AnalyzeSequence(Sequence sequence, CancellationToken ctx)
    {
        _thresholds = new Vector4D<byte>(ThresholdRed, ThresholdGreen, ThresholdBlue, ThresholdGamma);
        _sequence = sequence;
        _ctx = ctx;
        return true;
    }


    public float GetLineValue(long line)
    {
        if (_sequence is null || _ctx is null)
        {
            throw new InvalidStateException("Data must first be initialized!");
        }
        if(line < 0 || line >= _sequence.Shape.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(line));
        }

        return AnalyzeLine(line);
    }
    
    private float AnalyzeLine(long line)
    {
        float sum;
        
        sum = 0.0f;
        for(long j = 0; j < _sequence!.Shape.Width; j++)
        {
            var renderedData = _renderer!.RenderPixel(_sequence, j, line);
            Vector4D<byte> other = new Vector4D<byte>(ref renderedData);
            if(_thresholds.EntrywiseGreaterEqual(other))
            {
                sum += 1;
            }
        }
        return sum;
    }

    public string GetName()
    {
        return "Pixel Threshold Algorithm";
    }

    public ISaveControl GetSettingsPopupControl()
    {
        return new PixelThresholdSumAlgorithmConfigControlView(this);
    }
    

    public IMinimapAlgorithm Clone()
    {
        return new RenderedPixelThresholdAlgorithm { _renderer = _renderer , ThresholdRed = ThresholdRed, ThresholdGreen = ThresholdGreen, ThresholdBlue = ThresholdBlue, ThresholdGamma = ThresholdGamma};
    }

    public void SetRenderer(SequenceRenderer renderer)
    {
        _renderer = renderer;
    }

    internal class Vector4D<T> where T : IComparable<T>
    {
        private T[] Entries { get;  }

        public Vector4D(T r, T g, T b, T gamma)
        {
            Entries = new T[4];
            Entries[1] = r;
            Entries[2] = g;
            Entries[3] = b;
            Entries[0] = gamma;
        }
        public Vector4D(ref T[] data) {
            Entries = data;
        }
        public bool EntrywiseGreaterEqual(Vector4D<T> other)
        {
            for(int i = 0; i < 4; i++)
            {
                if(Entries[i].CompareTo(other.Entries[i]) > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}