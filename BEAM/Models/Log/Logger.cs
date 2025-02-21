using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Models.Log;

/// <summary>
/// The logger.
/// This Logger logs to a file by default. Other sinks can be added by observing the log entries.
/// </summary>
public class Logger : ObservableObject, ILog
{
    private static Logger? _instance;
    
    private readonly string _pathToLogFile;
    private LogLevel _logLevel;
    private LogEvent _logEvent;

    private readonly ObservableCollection<LogEntry> _logEntries;

    private Logger(string pathToLogFile)
    {
        _logEntries = [];
        _pathToLogFile = pathToLogFile;
        if (!File.Exists(pathToLogFile))
        {
            _CreateNewLogFile(pathToLogFile);
        }
    }

    /// <summary>
    /// Sets up the logger with a path to a log file.
    /// </summary>
    /// <param name="pathToLogFile">The path to the written log file</param>
    /// <returns></returns>
    public static Logger Init(string? pathToLogFile = null)
    {
        pathToLogFile ??= "log.txt";

        _instance = new Logger(pathToLogFile);
        return _instance;
    }

    /// <summary>
    /// Returns the singleton instance of the logger.
    /// </summary>
    /// <returns>The instance of the logger</returns>
    /// <exception cref="NullReferenceException">When the Init method has not been called before</exception>
    public static Logger GetInstance()
    {
        if (_instance is null) throw new NullReferenceException("The logger has not been initialized yet");
        return _instance;
    }

    public void Error(LogEvent occuredEvent, string logMessage)
    {
        _logLevel = LogLevel.Error;
        _logEvent = occuredEvent;
        Write("ERROR! --> " + occuredEvent + ": " + logMessage);
    }

    public void Warning(LogEvent occuredEvent, string logMessage)
    {
        _logLevel = LogLevel.Warning;
        _logEvent = occuredEvent;
        Write("Warning --> " + occuredEvent + ": " + logMessage);
    }

    public void Debug(LogEvent occuredEvent, string logMessage)
    {
        _logLevel = LogLevel.Debug;
        _logEvent = occuredEvent;
        Write("Debug --> " + occuredEvent + ": " + logMessage);
    }

    public void Info(LogEvent occuredEvent, string logMessage)
    {
        _logLevel = LogLevel.Info;
        _logEvent = occuredEvent;
        Write("Info: " + occuredEvent + ": " + logMessage);
    }

    public void LogMessage(string logMessage)
    {
        _logLevel = LogLevel.Info;
        _logEvent = LogEvent.BasicMessage;
        Write("Unspecified Log: " + logMessage);
    }

    private static void _CreateNewLogFile(string pathToLogFile)
    {
        using var fs = new FileStream(pathToLogFile, FileMode.CreateNew);
        using var w = new BinaryWriter(fs);

        w.Write("New log file created at: " + DateTime.Now + "\n");
    }

    private void Write(string message)
    {
        using (var outputFile = new StreamWriter(_pathToLogFile, true))
        {
            outputFile.WriteLine(DateTime.Now + " " + message);
        }

        _logEntries.Add(new LogEntry(_logLevel, Enum.GetName(_logEvent)!, message));
    }

    /// <summary>
    /// Clears the log entries list.
    /// </summary>
    public void ClearEntries()
    {
        _logEntries.Clear();
    }

    /// <summary>
    /// Gets an observable reference to the log entry list.
    /// </summary>
    /// <returns>The log entries</returns>
    public ObservableCollection<LogEntry> GetLogEntries()
    {
        return _logEntries;
    }
}