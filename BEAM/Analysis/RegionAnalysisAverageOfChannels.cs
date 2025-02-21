using System;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.Profiling;
using BEAM.ViewModels;
using ScottPlot;

namespace BEAM.Analysis;

/// <summary>
/// Class implementing methods for calculating the average of the Channel values of all pixels in a region.
/// Returns a plot depicting the average of the pixels in the region for each channels.
/// </summary>
public class RegionAnalysisAverageOfChannels : Analysis
{
    private const string Name = "Region Analysis Average";

    private double[] _sumChannels = [];  // sum of channel values
    private Coordinate2D _topLeft;
    private Coordinate2D _bottomRight;
    private int _amountChannels;

    public override Plot Analyze(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, ISequence sequence)
    {
        using var _ = Timer.Start("Region analysis (avg of channels)");
        _topLeft =
            new Coordinate2D((long) Math.Min(pointerPressedPoint.Row, pointerReleasedPoint.Row),
                (long) Math.Min(pointerPressedPoint.Column, pointerReleasedPoint.Column));
        
        _bottomRight =
            new Coordinate2D((long) Math.Max(pointerPressedPoint.Row, pointerReleasedPoint.Row),
                (long) Math.Max(pointerPressedPoint.Column, pointerReleasedPoint.Column));

        _amountChannels = sequence.Shape.Channels;
        
        // Catch trivial case of only one pixel selected
        if (Math.Abs(_AmountPixels() - 1) < 0.001)
        {
            _sumChannels = new double[_amountChannels];
            return PlotCreator.CreateFormattedBarPlot(_sumChannels);
        }
        
        // Calculate the average and store them in _sumChannels
        _CalculateResult(sequence);

        return PlotCreator.CreateFormattedBarPlot(_sumChannels);
    }
    
    
    
    /// <summary>
    /// Calculates the average of the channels in the region and stores the result in _sumChannels
    /// </summary>
    /// <param name="sequence"></param>
    private void _CalculateResult(ISequence sequence)
    {
        _sumChannels = new double[_amountChannels];

        // fill _sumChannels(Squared) with the correct values given in the sequence
        for (var row = _topLeft.Row; row <= _bottomRight.Row; row++)
        {
            for (var column = _topLeft.Column; column <= _bottomRight.Column; column++)
            {
                _UpdateWithPixel(sequence.GetPixel((long)column, (long)row));
            }
        }
        
        _calculateAverage();
    }

    
    /// <summary>
    /// Update _sumChannels with the given pixel.
    /// </summary>
    /// <param name="pixel"></param>
    private void _UpdateWithPixel(double[] pixel)
    {
        for (int channel = 0; channel < _amountChannels; channel++)
        {
            _sumChannels[channel] += pixel[channel];
        }
    }

    /// <summary>
    /// Calculate the amount of pixels in the given region.
    /// </summary>
    private double _AmountPixels()
    {
        return (_bottomRight.Row - _topLeft.Row + 1) * (_bottomRight.Column - _topLeft.Column + 1);
    }

    /// <summary>
    /// Convert _sumChannels entries to the means of the values of the channels.
    /// </summary>
    private void _calculateAverage()
    {
        var amountPixels = _AmountPixels();

        for (int i = 0; i < _amountChannels; i++)
        {
            _sumChannels[i] /= amountPixels;
        }
    }

    public override string ToString()
    {
        return Name;
    }

}