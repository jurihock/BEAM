namespace BEAM.ViewModels.LogViewModel;

public class LogViewModel
{
    private string lastestLog;
    private LogLevel logLevel;
    
    public void Update(string lastestLog, LogLevel logLevel)
    {
        this.lastestLog = lastestLog;
        this.logLevel = logLevel;
    }
    
    public string GetLastestLog()
    {
        return lastestLog;
    }
    
    public LogLevel GetLogLevel()
    {
        return logLevel;
    }

    private void NotifyView()
    {
        
    }
}