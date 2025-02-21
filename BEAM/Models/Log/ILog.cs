namespace BEAM.Models.Log;

/// <summary>
/// Interface for logging events.
/// </summary>
public interface ILog
{
    /// <summary>
    /// Logs an error.
    /// </summary>
    /// <param name="occuredEvent">The occured event</param>
    /// <param name="logMessage">The log message</param>
    public void Error(LogEvent occuredEvent, string logMessage);

    /// <summary>
    /// Logs a warning.
    /// </summary>
    /// <param name="occuredEvent">The occured event</param>
    /// <param name="logMessage">The log message</param>
    public void Warning(LogEvent occuredEvent,string logMessage);

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="occuredEvent">The occured event</param>
    /// <param name="logMessage">The log message</param>
    public void Debug(LogEvent occuredEvent,string logMessage);

    /// <summary>
    /// Logs an info message.
    /// </summary>
    /// <param name="occuredEvent">The occured event</param>
    /// <param name="logMessage">The log message</param>
    public void Info(LogEvent occuredEvent,string logMessage);

    /// <summary>
    /// Logs a message without a specified severity
    /// </summary>
    /// <param name="logMessage">The log message</param>
    public void LogMessage(string logMessage);
}