using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using BEAM.ViewModels;
using BEAM.ViewModels.LogViewModel;

namespace BEAM.Models.LoggerModels;

public class Logger :  ILog
{
    private static Logger _instance;
    
    private string _pathToLogFile;
    private string _lastLogMessage;
    private LogLevel _logLevel;
    private StatusBarViewModel _StatusBarViewModel;
    private Logger(string pathToLogFile)
    {
        _pathToLogFile = pathToLogFile;
        _StatusBarViewModel = StatusBarViewModel.GetInstance();
        if (!File.Exists(pathToLogFile))
        {
            CreateNewLogFile(pathToLogFile);
        }
        
    }

    public static Logger getInstance(string pathToLogFile)
    {
        return _instance ?? (_instance = new Logger(pathToLogFile));
    }
    
    public void Error(LogEvent occuredEvent)
    {
        _logLevel = LogLevel.Error;
        Write("ERROR! --> " + occuredEvent);
    }

    public void Warning(LogEvent occuredEvent)
    {
        _logLevel = LogLevel.Warning;
        Write("Warning --> " + occuredEvent);
    }

    public void Debug(LogEvent occuredEvent)
    {
        _logLevel = LogLevel.Debug;
        Write("Debug -->  " + occuredEvent);
    }

    public void Info(LogEvent occuredEvent)
    {
        _logLevel = LogLevel.Info;
        Write("Info: " + occuredEvent);
    }

    public void Error(LogEvent occuredEvent, string logMessage)
    {
        _logLevel = LogLevel.Error;
        Write("ERROR! --> " + occuredEvent + ": " + logMessage);
    }

    public void Warning(LogEvent occuredEvent, string logMessage)
    {
        _logLevel = LogLevel.Warning;
        Write("Warning --> " + occuredEvent + ": " + logMessage);
    }

    public void Debug(LogEvent occuredEvent, string logMessage)
    {
        _logLevel = LogLevel.Debug;
        Write("Debug --> " + occuredEvent + ": " + logMessage);
    }

    public void Info(LogEvent occuredEvent, string logMessage)
    {
        _logLevel = LogLevel.Info;
        Write("Info: " + occuredEvent + ": " + logMessage);
    }

    public void LogMessage(string logMessage)
    {
        _logLevel = LogLevel.Info;
        Write("Unspecified Log: " + logMessage);
    }
    
    private void CreateNewLogFile(string pathToLogFile)
    {
        using (FileStream fs = new FileStream(pathToLogFile, FileMode.CreateNew))
        {
            using (BinaryWriter w = new BinaryWriter(fs))
            {
                w.Write("New log file created at: " + DateTime.Now.ToString() + "\n");
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
        _StatusBarViewModel.Clear();
    }
}