using System;
using BEAM.ViewModels.LogViewModel;

namespace BEAM.Views;

public class LogView
{

    private LogLevel _logLevel;
    private string _lastestLog;
    private LogViewModel _logViewModel;
    
    public void UpdateLogWindow()
    {
        _lastestLog = _logViewModel.GetLastestLog();
        _logLevel = _logViewModel.GetLogLevel();
    }
   
}