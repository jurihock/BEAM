// File: BEAM.Tests/DummyMinimapAlgorithmTests.cs

using BEAM.Image;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.ImageSequence;

namespace BEAM.Tests.Minimap;

public class DummyMinimapAlgorithmTests
{
    private class DummySequence : ISequence
    {
        private ImageShape _shape = new ImageShape(100, 200, 4);
        double IImage.GetPixel(long x, long y, int channel)
        {
            return GetPixel(x, y, channel);
        }

        public double[] GetPixel(long x, long y)
        {
            throw new NotImplementedException();
        }

        public double[] GetPixel(long x, long y, int[] channels)
        {
            throw new NotImplementedException();
        }

        public LineImage GetPixelLineData(long line, int[] channels)
        {
            throw new NotImplementedException();
        }

        LineImage IImage.GetPixelLineData(long[] xs, long line, int[] channels)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        ImageShape IImage.Shape => _shape;

        public byte GetPixel(long x, long y, int channel) => 128;
        public LineImage GetPixelLineData(long[] xs, long y, int[] channels)
        {
            return new DummyPixelLineData((new double[xs.Length][]));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    
    private class DummyPixelLineData : LineImage
    {
        private readonly int _width;
        public DummyPixelLineData(double[][] data) : base(data)
        { _width = data.Length; }
        public byte GetPixel(int x, int channel, int dummy) => 128;
    }

    [Fact]
    public void AnalyzeSequence_ShouldReturnTrue()
    {
        var algorithm = new DummyMinimapAlgorithm();
        var sequence = new DummySequence();
        bool result = algorithm.AnalyzeSequence(sequence, CancellationToken.None);
        Assert.True(result);
    }

    [Fact]
    public void GetLineValue_ShouldReturnDoubleValue()
    {
        var algorithm = new DummyMinimapAlgorithm();
        double value = algorithm.GetLineValue(10);
        // Since the value comes from Random, we only check that it is within 0 and 1.
        Assert.InRange(value, 0, 1);
    }

    [Fact]
    public void GetName_ShouldReturnCorrectName()
    {
        var algorithm = new DummyMinimapAlgorithm();
        Assert.Equal("Dummy Algorithm", algorithm.GetName());
    }

    [Fact]
    public void Clone_ShouldReturnNewInstance()
    {
        var algorithm = new DummyMinimapAlgorithm();
        var clone = algorithm.Clone();
        Assert.IsType<DummyMinimapAlgorithm>(clone);
        Assert.NotSame(algorithm, clone);
    }
}