using System;
using System.Threading;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

/// <summary>
/// An algorithm for <see cref="PlotMinimap"/>'s which counts the number of rendered pixels within a line which
/// are greater or equal in value than a defined base pixel in every channel.
/// </summary>
public class RenderedPixelThresholdAlgorithm : IMinimapAlgorithm
{

    public byte ThresholdRed { get; set; } = 25;
    public byte ThresholdBlue { get; set; } = 25;
    public byte ThresholdGreen { get; set; } = 25;
    public byte ThresholdAlpha {get; set;} = 255 ;
    private ISequence? _sequence;
    private CancellationToken? _ctx;
    private SequenceRenderer? _renderer;
    Vector4D<byte> _thresholds = new Vector4D<byte>(25, 25, 25, 255);
    public bool AnalyzeSequence(ISequence sequence, CancellationToken ctx)
    {
        _thresholds = new Vector4D<byte>(ThresholdBlue, ThresholdGreen, ThresholdRed, ThresholdAlpha);
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

    public SaveUserControl GetSettingsPopupControl()
    {
        return new PixelThresholdSumAlgorithmConfigControlView(this);
    }
    

    public IMinimapAlgorithm Clone()
    {
        return new RenderedPixelThresholdAlgorithm { _renderer = _renderer , ThresholdRed = ThresholdRed, ThresholdGreen = ThresholdGreen, ThresholdBlue = ThresholdBlue, ThresholdAlpha = ThresholdAlpha};
    }

    public void SetRenderer(SequenceRenderer renderer)
    {
        _renderer = renderer;
    }

    /// <summary>
    /// A Vector with 4 entries.
    /// </summary>
    /// <typeparam name="T">The vector entry's data type.</typeparam>
    internal class Vector4D<T> where T : IComparable<T>
    {
        private T[] Entries { get;  }

        /// <summary>
        /// Creates a new vector from four separate entries.
        /// </summary>
        /// <param name="entry1">The first entry.</param>
        /// <param name="entry2">The second entry.</param>
        /// <param name="entry3">The third entry.</param>
        /// <param name="entry4">The fourth entry.</param>
        public Vector4D(T entry1, T entry2, T entry3, T entry4)
        {
            Entries = new T[4];
            Entries[1] = entry1;
            Entries[2] = entry2;
            Entries[3] = entry3;
            Entries[0] = entry4;
        }
        /// <summary>
        /// Creates a new vector from an existing array of Data.
        /// This array must therefore have length 4.
        /// </summary>
        /// <param name="data">A reference to the data array.</param>
        public Vector4D(ref T[] data) {
            Entries = data;
        }
        
        /// <summary>
        /// Compares to Vectors entrywise. Returns true if all of this instance's entries are greater or equal
        /// to other instanc's values.
        /// </summary>
        /// <param name="other">The Vector to compare against.</param>
        /// <returns>True if this instance is greater or equal in every entry, false else-wise.</returns>
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