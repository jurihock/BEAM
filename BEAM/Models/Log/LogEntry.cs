using System;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Models.Log;

/// <summary>
/// A simple model of a single log event.
/// </summary>
/// <param name="level">The level the event occured at</param>
/// <param name="occuredEvent">The name of the occured event</param>
/// <param name="message">The additional log message</param>
public partial class LogEntry(LogLevel level, string occuredEvent, string message) : ViewModelBase
{
    [ObservableProperty] public partial LogLevel Level { get; set; } = level;
    
    // not nullable since LogLevel is not
    public string LevelStr => Enum.GetName(Level)!.ToUpper();

    [ObservableProperty] public partial string Event { get; set; } = occuredEvent;
    [ObservableProperty] public partial string Message { get; set; } = message;
}