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

/// <summary>
/// An algorithm for <see cref="PlotMinimap"/>'s which counts the number of rendered pixels within a line whose specified channel's
/// value is greater than or equal in value to a defined base value.
/// </summary>
public class RenderedChannelThresholdAlgorithm : IMinimapAlgorithm
{
    /// <summary>
    /// An algorithm for <see cref="PlotMinimap"/>'s which counts the number of rendered pixels within a line which
    /// are greater or equal in value than a defined base pixel in every channel.
    /// </summary>
    public int Channel { get; set; }

    /// <summary>
    /// Gets or sets the threshold value (0-255) for the selected channel.
    /// </summary>
    public byte ChannelThreshold { get; set; } = 25;
    private ISequence? _sequence;
    private CancellationToken? _ctx;
    private SequenceRenderer? _renderer;
    private BGR _thresholds;


    /// <summary>
    /// Prepares the algorithm for sequence analysis by initializing thresholds and storing sequence data.
    /// </summary>
    /// <param name="sequence">The image sequence to analyze.</param>
    /// <param name="ctx">Cancellation token for interrupting the analysis.</param>
    /// <returns>True if initialization was successful.</returns>
    public bool AnalyzeSequence(ISequence sequence, CancellationToken ctx)
    {
        byte[] data = [0, 0, 0];
        data[Channel] = ChannelThreshold;
        _thresholds = new BGR(data);
        _sequence = sequence;
        _ctx = ctx;
        return true;
    }

    /// <summary>
    /// Initializes a new instance of the RenderedChannelThresholdAlgorithm.
    /// Sets up initial threshold values for the selected channel.
    /// </summary>
    public RenderedChannelThresholdAlgorithm()
    {
        byte[] data = new byte[] { 0, 0, 0 };
        data[Channel] = ChannelThreshold;
        _thresholds = new BGR(data);
    }



    /// <summary>
    /// Computes the value for a specific line in the sequence based on threshold comparison.
    /// </summary>
    /// <param name="line">The line index to analyze.</param>
    /// <returns>The number of pixels meeting the threshold criteria.</returns>
    /// <exception cref="InvalidStateException">Thrown when sequence data is not initialized.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when line index is invalid.</exception>
    public double GetLineValue(long line)
    {
        if (_sequence is null || _ctx is null)
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

    /// <summary>
    /// Gets the display name of this algorithm.
    /// </summary>
    /// <returns>The algorithm's name.</returns>
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
        return new RenderedChannelThresholdAlgorithm { _renderer = _renderer, Channel = Channel, ChannelThreshold = ChannelThreshold };
    }

    public void SetRenderer(SequenceRenderer renderer)
    {
        _renderer = renderer;
    }
}