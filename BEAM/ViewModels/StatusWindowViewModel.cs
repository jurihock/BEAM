using System.Collections.ObjectModel;
using BEAM.Models.LoggerModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

public partial class StatusWindowViewModel : ViewModelBase
{
    public ObservableCollection<LogEntry> StatusList;
    
    public StatusWindowViewModel()
    {
        StatusList = new ObservableCollection<LogEntry>();
    }
}