using System.Collections.Generic;
using System.Collections.ObjectModel;
using BEAM.Models.LoggerModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

public partial class StatusWindowViewModel : ViewModelBase
{
    [ObservableProperty] public partial List<LogEntry> StatusList { get; set; }

    public StatusWindowViewModel()
    {
        StatusList = new List<LogEntry>();
    }
}