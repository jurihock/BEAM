using System.Collections.Generic;
using Avalonia.Interactivity;
using ScottPlot.Avalonia;

namespace BEAM.ImageSequence.Synchronization.Manipulators;

public static class EventSourceMapper
{
    private static Queue<RoutedEventArgs> _eventQueue = new();
    private static Dictionary<RoutedEventArgs, AvaPlot?> _sourcePlots = new();

    public static void AddEventSource(RoutedEventArgs eventSource, AvaPlot? plot)
    {
        if (_eventQueue.Count >= 100)
        {
            var e = _eventQueue.Dequeue();
            _sourcePlots.Remove(e);
        }
        _eventQueue.Enqueue(eventSource);
        _sourcePlots.Add(eventSource, plot);
    }
    
    public static bool IsSource(RoutedEventArgs e, AvaPlot plot) {
        if (_sourcePlots.TryGetValue(e, out var source))
        {
            return source != null && source.Equals(plot);
        }
        return false;
    }

    public static void AddIfNotExists(RoutedEventArgs e, AvaPlot plot)
    {
        if (!_sourcePlots.ContainsKey(e))
        {
            AddEventSource(e, plot);
        }
    }
    
    public static void Clear()
    {
        _eventQueue.Clear();
        _sourcePlots.Clear();
    }
}