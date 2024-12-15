using System;
using System.Collections.Generic;
using System.IO;
using BEAM.ViewModels;

namespace BEAM.Log;

public class Logger :  ILog
{
    private static Logger? _instance;
    
    private string _pathToLogFile;
    private string _lastLogMessage;
    private LogLevel _logLevel;
    private LogEvent _logEvent;
    private StatusBarViewModel _StatusBarViewModel;

    private List<LogEntry> _logEntries;
    
    private Logger(string pathToLogFile)
    {
        _logEntries = new List<LogEntry>();
        _pathToLogFile = pathToLogFile;
        _StatusBarViewModel = StatusBarViewModel.GetInstance();
        if (!File.Exists(pathToLogFile))
        {
            CreateNewLogFile(pathToLogFile);
        }
    }

    public static Logger Init(string? pathToLogFile=null)
    {
        if (pathToLogFile is null)
        {
            pathToLogFile = "log.txt";
        }

        _instance = new Logger(pathToLogFile);
        return _instance;
    }

    public static Logger GetInstance()
    {
        return _instance ?? Init();
    }
    
    public void Error(LogEvent occuredEvent)
    {
        _logLevel = LogLevel.Error;
        _logEvent = occuredEvent;
        Write("ERROR! --> " + occuredEvent);
    }

    public void Warning(LogEvent occuredEvent)
    {
        _logLevel = LogLevel.Warning;
        _logEvent = occuredEvent;
        Write("Warning --> " + occuredEvent);
    }

    public void Debug(LogEvent occuredEvent)
    {
        _logLevel = LogLevel.Debug;
        _logEvent = occuredEvent;
        Write("Debug -->  " + occuredEvent);
    }

    public void Info(LogEvent occuredEvent)
    {
        _logLevel = LogLevel.Info;
        _logEvent = occuredEvent;
        Write("Info: " + occuredEvent);
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
    
    private void CreateNewLogFile(string pathToLogFile)
    {
        using (FileStream fs = new FileStream(pathToLogFile, FileMode.CreateNew))
        {
            using (BinaryWriter w = new BinaryWriter(fs))
            {
                w.Write("New log file created at: " + DateTime.Now + "\n");
            }
        }
            
    }
    
    private void Write(string message)
    {
        _lastLogMessage = message;
        using (StreamWriter outputFile = new StreamWriter(_pathToLogFile, true))
        {
            outputFile.WriteLine(DateTime.Now + " " +message);
        }
        _logEntries.Add(new LogEntry(Enum.GetName(_logLevel).ToUpper(), Enum.GetName(_logEvent), message));
        if (_logLevel == LogLevel.Error)
        {
            _StatusBarViewModel.AddError(message);
        }
        else if (_logLevel == LogLevel.Warning)
        {
            _StatusBarViewModel.AddWarning(message);
        }
        else
        {
            _StatusBarViewModel.AddInfo(message);
        }
    }
    
    public string GetLastLogMessage()
    {
        return _lastLogMessage;
    }
    
    public void ClearStatusBar()
    {
        _logEntries.Clear();
        _StatusBarViewModel.Clear();
    }
    
    public List<LogEntry> GetLogEntries()
    {
        return _logEntries;
    }
}