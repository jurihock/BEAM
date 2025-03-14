using BEAM.Image;
using BEAM.ImageSequence;

namespace BEAM.Tests.ImageSequence;

[Collection("GlobalTests")]
public class SequenceTests
{
    class ValueSequence(int width, int height, int channels, double value) : ISequence
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

        public LineImage GetPixelLineData(long line, int[] channels)
        {
            var data = new double[channels.Length][];
            for (int i = 0; i < channels.Length; i++)
            {
                data[i] = new double[Shape.Width];
                for (var j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = value;
                }
            }

            return new LineImage(data);
        }

        public LineImage GetPixelLineData(long[] xs, long line, int[] channels)
        {
            var data = new double[channels.Length][];
            for (int i = 0; i < channels.Length; i++)
            {
                data[i] = new double[xs.Length];
                for (var j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = value;
                }
            }

            return new LineImage(data);
        }

        public string GetName() => "ValueSequence";
    }

    /// <summary>
    /// Channel 0: x-dir
    /// Channel 1: y-dir
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    class GradientSequence(int width, int height, double start, double end) : ISequence
    {
        private double LerpX(long pos)
        {
            var t = pos / (double)Shape.Width;
            return start * (1 - t) + (end * t);
        }
        private double LerpY(long pos)
        {
            var t = pos / (double)Shape.Width;
            return start * (1 - t) + (end * t);
        }

        public void Dispose()
        {
        }

        public ImageShape Shape => new ImageShape(width, height, 2);

        public double GetPixel(long x, long y, int channel)
        {
            return channel == 0 ? LerpX(x) : LerpY(y);
        }

        public double[] GetPixel(long x, long y)
        {
            return [LerpX(x), LerpY(y)];
        }

        public double[] GetPixel(long x, long y, int[] channels)
        {
            var data = new double[channels.Length];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = GetPixel(x, y, channels[i]);
            }

            return data;
        }

        public LineImage GetPixelLineData(long line, int[] channels)
        {
            var data = new double[channels.Length][];
            for (var i = 0; i < channels.Length; i++)
            {
                data[i] = new double[Shape.Width];
                for (var j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = GetPixel(j, line, channels[i]);
                }
            }

            return new LineImage(data);
        }

        public LineImage GetPixelLineData(long[] xs, long line, int[] channels)
        {
            var data = new double[channels.Length][];
            for (var i = 0; i < channels.Length; i++)
            {
                data[i] = new double[xs.Length];
                for (var j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = GetPixel(xs[j], line, channels[i]);
                }
            }

            return new LineImage(data);
        }

        public string GetName()
        {
            return "GradientSequence";
        }
    }

    [Fact]
    public void CutCorrectLength()
    {
        var seq = new ValueSequence(100, 100, 3, 0);
        var cut = new CutSequence(seq.GetName(), 15, 70, seq);
        Assert.Equal(55, cut.Shape.Height);
    }

    [Fact]
    public void CutCorrectLength_NoOffset()
    {
        var seq = new ValueSequence(100, 100, 3, 0);
        var cut = new CutSequence(seq.GetName(), 0, 100, seq);
        Assert.Equal(100, cut.Shape.Height);
    }

    [Fact]
    public void CutCorrectLength_All()
    {
        var seq = new ValueSequence(100, 100, 3, 0);
        var cut = new CutSequence(seq.GetName(), 100, 100, seq);
        Assert.Equal(0, cut.Shape.Height);
    }

    [Fact]
    public void ScaledSequence()
    {
        var seq = new ValueSequence(100, 100, 3, 0);
        var transformed = new TransformedSequence(seq);
        transformed.ScaleX = 2;
        transformed.ScaleY = 2;
        Assert.Equal(200, transformed.Shape.Width);
        Assert.Equal(200, transformed.Shape.Height);
    }

    [Fact]
    public void ScaledSequence_ToZero()
    {
        var seq = new ValueSequence(100, 100, 3, 0);
        var transformed = new TransformedSequence(seq);
        transformed.ScaleX = 0;
        transformed.ScaleY = 0;
        Assert.Equal(0, transformed.Shape.Width);
        Assert.Equal(0, transformed.Shape.Height);
    }

    [Fact]
    public void CutCorrectColors()
    {
        var seq = new GradientSequence(100, 100, 0, 100);
        var cut = new CutSequence(seq.GetName(), 10, 90, seq);
        Assert.Equal(10, cut.GetPixel(0, 0)[1]);
        Assert.Equal(80, cut.GetPixel(0, 70)[1]);
    }

    [Fact]
    public void TransformCorrectColors()
    {
        var seq = new GradientSequence(100, 100, 0, 100);
        Assert.Equal(0, seq.GetPixel(0, 0)[0]);
        Assert.Equal(1, seq.GetPixel(1, 0)[0]);
        var transformed = new TransformedSequence(seq);
        transformed.ScaleX = 2;
        transformed.ScaleY = 2;
        Assert.Equal(0, transformed.GetPixel(0, 0)[0]);
        Assert.Equal(0, transformed.GetPixel(1, 0)[0]);
        Assert.Equal(1, transformed.GetPixel(2, 0)[0]);
        Assert.Equal(1, transformed.GetPixel(3, 0)[0]);

        Assert.Equal(99, transformed.GetPixel(198, 0)[0]);
        Assert.Equal(99, transformed.GetPixel(199, 0)[0]);
    }
}