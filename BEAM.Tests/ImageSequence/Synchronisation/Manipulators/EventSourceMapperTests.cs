using BEAM.ImageSequence.Synchronization.Manipulators;

namespace BEAM.Tests.ImageSequence.Synchronisation.Manipulators;

using Avalonia.Interactivity;
using ScottPlot.Avalonia;
using Xunit;

public class EventSourceMapperTests
{
    [Fact]
    public void AddEventSource_AddsEventAndSource()
    {
        var eventSource = new RoutedEventArgs();
        var plot = new AvaPlot();

        EventSourceMapper.AddEventSource(eventSource, plot);

        Assert.True(EventSourceMapper.IsSource(eventSource, plot));
    }

    [Fact]
    public void AddEventSource_LimitExceeded()
    {
        for (int i = 0; i < 100; i++)
        {
            EventSourceMapper.AddEventSource(new RoutedEventArgs(), new AvaPlot());
        }

        var newEvent = new RoutedEventArgs();
        var newPlot = new AvaPlot();
        EventSourceMapper.AddEventSource(newEvent, newPlot);

        Assert.False(EventSourceMapper.IsSource(new RoutedEventArgs(), new AvaPlot()));
        Assert.True(EventSourceMapper.IsSource(newEvent, newPlot));
        
        for (int i = 0; i < 100; i++)
        {
            EventSourceMapper.AddEventSource(new RoutedEventArgs(), new AvaPlot());
        }
        
        newEvent = new RoutedEventArgs();
        newPlot = new AvaPlot();
        EventSourceMapper.AddEventSource(newEvent, newPlot);

        Assert.False(EventSourceMapper.IsSource(new RoutedEventArgs(), new AvaPlot()));
        Assert.True(EventSourceMapper.IsSource(newEvent, newPlot));
    }

    [Fact]
    public void IsSource_ReturnsFalse_WhenEventNotMapped()
    {
        var eventSource = new RoutedEventArgs();
        var plot = new AvaPlot();

        Assert.False(EventSourceMapper.IsSource(eventSource, plot));
    }

    [Fact]
    public void AddIfNotExists_AddsEventAndSource_WhenEventNotMapped()
    {
        var eventSource = new RoutedEventArgs();
        var plot = new AvaPlot();

        EventSourceMapper.AddIfNotExists(eventSource, plot);

        Assert.True(EventSourceMapper.IsSource(eventSource, plot));
    }

    [Fact]
    public void AddIfNotExists_DoesNotAddEvent_WhenEventAlreadyMapped()
    {
        var eventSource = new RoutedEventArgs();
        var plot1 = new AvaPlot();
        var plot2 = new AvaPlot();

        EventSourceMapper.AddEventSource(eventSource, plot1);
        EventSourceMapper.AddIfNotExists(eventSource, plot2);

        Assert.True(EventSourceMapper.IsSource(eventSource, plot1));
        Assert.False(EventSourceMapper.IsSource(eventSource, plot2));
    }

    [Fact]
    public void Clear_RemovesAllEventsAndSources()
    {
        var eventSource = new RoutedEventArgs();
        var plot = new AvaPlot();

        EventSourceMapper.AddEventSource(eventSource, plot);
        EventSourceMapper.Clear();

        Assert.False(EventSourceMapper.IsSource(eventSource, plot));
    }
}