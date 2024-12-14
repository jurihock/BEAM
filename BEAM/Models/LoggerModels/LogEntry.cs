using BEAM.ViewModels;
using BEAM.ViewModels.LogViewModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Models.LoggerModels;

public partial class LogEntry(string level, string occuredEvent, string message) : ViewModelBase
{
    [ObservableProperty] public partial string? Level { get; set; } = level;
    [ObservableProperty] public partial string? Event { get; set; } = occuredEvent;
    [ObservableProperty] public partial string? Message { get; set; } = message;
}