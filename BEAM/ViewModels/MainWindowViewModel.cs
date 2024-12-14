using BEAM.Models.LoggerModels;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private Logger? _logger;
    public MainWindowViewModel()
    {
        _logger = new Logger("../../../../BEAM.Tests/loggerTests/testLogs/testLog.txt");
    }
    
    [RelayCommand]
    public void AddInfo()
    {
        _logger?.Info(LogEvent.OpenedFile);
    }
    [RelayCommand]
    public void AddWarning()
    {
        _logger?.Warning(LogEvent.UnknownFileFormat);
    }
    [RelayCommand]
    public void AddError()
    {
        _logger?.Error(LogEvent.FileNotFound);
    }
    
    [RelayCommand]
    public void ClearLog()
    {
        _logger?.ClearStatusBar();
    }
}