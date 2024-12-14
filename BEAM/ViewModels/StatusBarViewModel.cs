using System.IO.Pipelines;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

public partial class StatusBarViewModel : ViewModelBase
{
    private static StatusBarViewModel? _instance;
    
    private long _infoCounter = 0;
    private long _warningCounter = 0;
    private long _errorCounter = 0;
    [ObservableProperty] public partial bool InfoVisible { get; set; } = false;
    [ObservableProperty] public partial bool WarningVisible { get; set; } = false;
    [ObservableProperty] public partial bool ErrorVisible { get; set; } = false;
    [ObservableProperty] public partial string InfoText { get; set; } = "";
    [ObservableProperty] public partial string WarningText { get; set; } = "";
    [ObservableProperty] public partial string ErrorText { get; set; } = "";

    private StatusBarViewModel()
    {
        
    }

    public static StatusBarViewModel GetInstance()
    {
        return _instance ?? (_instance = new StatusBarViewModel());
    }
    
    public void AddInfo(string infoMessage)
    {
        _infoCounter += 1;
        InfoVisible = true;
        InfoText = _infoCounter >= 100 ? "99+ Info" : _infoCounter + " Info";
    }
    
    public void AddWarning(string warningMessage)
    {
        _warningCounter += 1;
        WarningVisible = true;
        WarningText = _warningCounter >= 100 ? "99+ Warnings" : _warningCounter + " Warnings";
    }
    
    public void AddError(string errorMessage)
    {
        _errorCounter += 1;
        ErrorText = _errorCounter >= 100 ? "99+ Errors" : _errorCounter + " Errors";
        ErrorVisible = true;
    }
    
    public void Clear()
    {
        _infoCounter = 0;
        _warningCounter = 0;
        _errorCounter = 0;
        InfoVisible = false;
        WarningVisible = false;
        ErrorVisible = false;
        InfoText = "";
        WarningText = "";
        ErrorText = "";
    }
}