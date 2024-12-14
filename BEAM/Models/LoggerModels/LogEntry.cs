using BEAM.ViewModels;
using BEAM.ViewModels.LogViewModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Models.LoggerModels;

public partial class LogEntry(LogLevel level, LogEvent occuredEvent, string message) : ViewModelBase
{
    [ObservableProperty] 
    public partial LogLevel  Level { get; set; }
    [ObservableProperty]
    public partial LogEvent Event { get; set; }
    [ObservableProperty]
    public partial string Message { get; set; }
}