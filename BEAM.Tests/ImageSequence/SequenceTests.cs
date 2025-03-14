using System.Buffers;
using BEAM.Image;
using BEAM.ImageSequence;

namespace BEAM.Tests.ImageSequence;

public class SequenceTests
{
    class MockSequence(int width, int height, int channels, double value) : ISequence
    {

        public void Dispose()
        {
        }

        public ImageShape Shape => new(width, height, channels);

        public double GetPixel(long x, long y, int channel) => value;
        public double[] GetPixel(long x, long y)
        {
            var data = new double[Shape.Channels];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = value;
            }

            return data;
        }

        public double[] GetPixel(long x, long y, int[] channels)
        {
            var data = new double[channels.Length];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = value;
            }

            return data;
        }

        public LineImage GetPixelLineData(long line, int[] channels, ArrayPool<double> pool)
        {
            var data = new double[channels.Length][];
            for (int i = 0; i < channels.Length; i++)
            {
                data[i] = pool.Rent((int) Shape.Width);
                for (var j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = value;
                }
            }

            return new LineImage(data, pool);
        }

        public LineImage GetPixelLineData(long[] xs, long line, int[] channels, ArrayPool<double> pool)
        {
            var data = new double[channels.Length][];
            for (int i = 0; i < channels.Length; i++)
            {
                data[i] = pool.Rent(xs.Length);
                for (var j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = value;
                }
            }

            return new LineImage(data, pool);
        }

        public string GetName() => "MockSequence";
    }
    
    [Fact]
    public void CutCorrectLength()
    {
        var seq = new MockSequence(100, 100, 3, 0);
        var cut = new CutSequence(seq.GetName(), 15, 70, seq);
        Assert.Equal(55, cut.Shape.Height);
    }

    [Fact]
    public void CutCorrectLength_NoOffset()
    {
        var seq = new MockSequence(100, 100, 3, 0);
        var cut = new CutSequence(seq.GetName(), 0, 100, seq);
        Assert.Equal(100, cut.Shape.Height);
    }

    [Fact]
    public void CutCorrectLength_All()
    {
        var seq = new MockSequence(100, 100, 3, 0);
        var cut = new CutSequence(seq.GetName(), 100, 100, seq);
        Assert.Equal(0, cut.Shape.Height);
    }

    [Fact]
    public void ScaledSequence()
    {
        var seq = new MockSequence(100, 100, 3, 0);
        var transformed = new TransformedSequence(seq);
        transformed.ScaleX = 2;
        transformed.ScaleY = 2;
        Assert.Equal(200, transformed.Shape.Width);
        Assert.Equal(200, transformed.Shape.Height);
    }

    [Fact]
    public void ScaledSequence_ToZero()
    {
        var seq = new MockSequence(100, 100, 3, 0);
        var transformed = new TransformedSequence(seq);
        transformed.ScaleX = 0;
        transformed.ScaleY = 0;
        Assert.Equal(0, transformed.Shape.Width);
        Assert.Equal(0, transformed.Shape.Height);
    }
}