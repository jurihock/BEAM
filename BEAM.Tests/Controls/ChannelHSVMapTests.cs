namespace BEAM.Tests.Controls;

using BEAM.Datatypes.Color;
using BEAM.Controls;
using Xunit;

[Collection("GlobalTests")]
public class ChannelHSVMapTests
{
    [Fact]
    public void GetColorHSV_ReturnsCorrectHSVColor_WhenChannelIsValid()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0.5) };
        var map = new ChannelHSVMap(channels);

        var color = map.GetColorHSV(0);

        Assert.Equal(0.5, color.H);
    }

    [Fact]
    public void GetColorBGR_ReturnsCorrectBGRColor_WhenChannelIsValid()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0.5) };
        var map = new ChannelHSVMap(channels);

        var color = map.GetColorBGR(0);

        Assert.Equal(0.5, color.ToHsv().H);
    }
    
    [Fact]
    public void GetColorBGR_ReturnsCorrectBGRColor_WhenChannelIsValid_WithBGRColor_Zero()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0) };
        var map = new ChannelHSVMap(channels);

        var color = map.GetColorBGR(0);

        Assert.Equal(color.R, 255);
        Assert.Equal(color.G, 0);
        Assert.Equal(color.B, 0);
    }
    
    [Fact]
    public void GetColorBGR_ReturnsCorrectBGRColor_WhenChannelIsValid_WithBGRColor_Max()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(1) };
        var map = new ChannelHSVMap(channels);

        var color = map.GetColorBGR(0);

        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void SetColor_SetsCorrectHSVValue_WhenCalledWithBGRColor()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0.5) };
        var map = new ChannelHSVMap(channels);
        var newColor = new BGR { R = 160, G = 160, B = 160 };

        map.SetColor(0, newColor);

        Assert.True(Math.Abs(0.627 - map.GetColorHSV(0).H) < 0.001);
        Assert.True(Math.Abs(1.0 - map.GetColorHSV(0).V) < 0.001);
        Assert.True(Math.Abs(1.0 - map.GetColorBGR(0).ToHsv().S) < 0.001);
    }
    
    

    [Fact]
    public void IsChannelUsed_ReturnsTrue_WhenChannelIsUsed()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0.5) { IsUsedForArgMax = true } };
        var map = new ChannelHSVMap(channels);

        var result = map.IsChannelUsed(0);

        Assert.True(result);
    }

    [Fact]
    public void IsChannelUsed_ReturnsFalse_WhenChannelIsNotUsed()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0.5) { IsUsedForArgMax = false } };
        var map = new ChannelHSVMap(channels);

        var result = map.IsChannelUsed(0);

        Assert.False(result);
    }

    [Fact]
    public void SetUsedChannels_SetsChannelUsageCorrectly()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0.5) };
        var map = new ChannelHSVMap(channels);

        map.SetUsedChannels(0, true);

        Assert.True(map.IsChannelUsed(0));
    }

    [Fact]
    public void GetAmountUsedChannels_ReturnsCorrectCount()
    {
        var channels = new ChannelToHSV[] 
        { 
            new ChannelToHSV(0.5) { IsUsedForArgMax = true },
            new ChannelToHSV(0.6) { IsUsedForArgMax = false },
            new ChannelToHSV(0.7) { IsUsedForArgMax = true }
        };
        var map = new ChannelHSVMap(channels);

        var count = map.GetAmountUsedChannels();

        Assert.Equal(2, count);
    }

    [Fact]
    public void GetUsedChannels_ReturnsCorrectIndices()
    {
        var channels = new ChannelToHSV[] 
        { 
            new ChannelToHSV(0.5) { IsUsedForArgMax = true },
            new ChannelToHSV(0.6) { IsUsedForArgMax = false },
            new ChannelToHSV(0.7) { IsUsedForArgMax = true }
        };
        var map = new ChannelHSVMap(channels);

        var usedChannels = map.GetUsedChannels();

        Assert.Equal(new int[] { 0, 2 }, usedChannels);
    }

    [Fact]
    public void Clone_CreatesExactCopyOfChannelHSVMap()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0.5) { IsUsedForArgMax = true } };
        var map = new ChannelHSVMap(channels);

        var clone = map.Clone();

        Assert.Equal(map.GetColorHSV(0).V, clone.GetColorHSV(0).V);
        Assert.Equal(map.IsChannelUsed(0), clone.IsChannelUsed(0));
    }

    [Fact]
    public void ToArray_ReturnsCorrectChannelArray()
    {
        var channels = new ChannelToHSV[] { new ChannelToHSV(0.5) };
        var map = new ChannelHSVMap(channels);

        var array = map.ToArray();

        Assert.Equal(channels, array);
    }
}