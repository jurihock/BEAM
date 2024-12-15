using System;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Log;

public partial class LogEntry(LogLevel level, string occuredEvent, string message) : ViewModelBase
{
    [ObservableProperty] public partial LogLevel Level { get; set; } = level;
    public string LevelStr {get => Enum.GetName(Level).ToUpper();}

    [ObservableProperty] public partial string? Event { get; set; } = occuredEvent;
    [ObservableProperty] public partial string? Message { get; set; } = message;
}