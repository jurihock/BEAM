using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BEAM.Log;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class StatusWindowViewModel : ViewModelBase
{
    private Logger? _logger;
    public ObservableCollection<LogEntry> StatusList { get; set; }

    public StatusWindowViewModel()
    {
        _logger = Logger.GetInstance();
        StatusList = _logger.GetLogEntries();
    }

    [RelayCommand]
    public void ClearStatus()
    {
        _logger.ClearStatusBar();
    }
}