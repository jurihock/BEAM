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

public class RenderedPixelAnyThresholdAlgorithm : IMinimapAlgorithm
{

    public byte ThresholdRed { get; set; } = 25;
    public byte ThresholdBlue { get; set; } = 25;
    public byte ThresholdGreen { get; set; } = 25;
    public byte ThresholdAlpha {get; set;} = 255 ;
    private ISequence? _sequence;
    private CancellationToken? _ctx;
    private SequenceRenderer? _renderer;
    private BGR _thresholds;
    public RenderedPixelAnyThresholdAlgorithm()
    {
        _thresholds = new BGR(ThresholdBlue, ThresholdGreen, ThresholdRed);
    }
    public bool AnalyzeSequence(ISequence sequence, CancellationToken ctx)
    {
        _thresholds = new BGR(ThresholdBlue, ThresholdGreen,  ThresholdRed);
        _sequence = sequence;
        _ctx = ctx;
        return true;
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
        for(long j = 0; j < _sequence!.Shape.Width; j++)
        {
            var renderedData = _renderer!.RenderPixel(_sequence, j, line);
            if(_thresholds.EntrywiseAnyGreater(renderedData))
            {
                sum += 1;
            }
            
        }
        return sum;
    }

    public string GetName()
    {
        return "Pixel Any Threshold Algorithm";
    }

    public SaveUserControl GetSettingsPopupControl()
    {
        return new PixelThresholdAnySumAlgorithmConfigControlView(this);
    }
    

    public IMinimapAlgorithm Clone()
    {
        return new RenderedPixelAnyThresholdAlgorithm { _renderer = _renderer , ThresholdRed = ThresholdRed, ThresholdGreen = ThresholdGreen, ThresholdBlue = ThresholdBlue, ThresholdAlpha = ThresholdAlpha};
    }

    public void SetRenderer(SequenceRenderer renderer)
    {
        _renderer = renderer;
    }
    
}