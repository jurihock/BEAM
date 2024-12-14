using BEAM.ViewModels.LogViewModel;

namespace BEAM.Models.LoggerModels;

public class LogEntry(LogLevel level, LogEvent occuredEvent, string message)
{
    public LogLevel Level;
    public LogEvent Event;
    public string Message;
}