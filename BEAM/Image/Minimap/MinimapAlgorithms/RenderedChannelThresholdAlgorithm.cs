using System;
using System.Threading;
using BEAM.Datatypes.Color;
using BEAM.Exceptions;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

public class RenderedChannelThresholdAlgorithm : IMinimapAlgorithm
{
    public int Channel  {get; set;}
    
    public byte ChannelThreshold { get; set; } = 25;
    private ISequence? _sequence;
    private CancellationToken? _ctx;
    private SequenceRenderer? _renderer;
    private BGR _thresholds;
    public bool AnalyzeSequence(ISequence sequence, CancellationToken ctx)
    {
        byte[] data = new byte[] {0, 0, 0};
        data[Channel] = ChannelThreshold;
        _thresholds = new BGR(data);
        _sequence = sequence;
        _ctx = ctx;
        return true;
    }

    public RenderedChannelThresholdAlgorithm()
    {
        byte[] data = new byte[] {0, 0, 0};
        data[Channel] = ChannelThreshold;
        _thresholds = new BGR(data);
    }


    public double GetLineValue(long line)
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
    
    private double AnalyzeLine(long line)
    {
        double sum = 0.0f;
        for (long j = 0; j < _sequence!.Shape.Width; j++)
        {
            var renderedData = _renderer!.RenderPixel(_sequence, j, line);
            if (renderedData.EntrywiseAllGreaterEqual(_thresholds))
            {
                sum += 1;
            }
        }
        return sum;
    }

    public string GetName()
    {
        return "Channel Threshold Algorithm";
    }

    public SaveUserControl GetSettingsPopupControl()
    {
        return new RenderedChannelThresholdView(this);
    }
    

    public IMinimapAlgorithm Clone()
    {
        return new RenderedChannelThresholdAlgorithm { _renderer = _renderer , Channel = Channel, ChannelThreshold = ChannelThreshold};
    }

    public void SetRenderer(SequenceRenderer renderer)
    {
        _renderer = renderer;
    }
}