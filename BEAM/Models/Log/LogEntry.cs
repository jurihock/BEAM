using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Log;

public partial class LogEntry(string level, string occuredEvent, string message) : ViewModelBase
{
    [ObservableProperty] public partial string? Level { get; set; } = level;
    [ObservableProperty] public partial string? Event { get; set; } = occuredEvent;
    [ObservableProperty] public partial string? Message { get; set; } = message;
}