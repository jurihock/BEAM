using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using BEAM.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using BEAM.Models.LoggerModels;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public partial string File { get; set; } = Configuration.StandardEnglish().FileMenu;
    [ObservableProperty] public partial string Open { get; set; } = Configuration.StandardEnglish().Open;
    [ObservableProperty] public partial string Exit { get; set; } = Configuration.StandardEnglish().Exit;
    [ObservableProperty] public partial string Edit { get; set; } = Configuration.StandardEnglish().Edit;
    [ObservableProperty] public partial string Paste { get; set; } = Configuration.StandardEnglish().Paste;
    [ObservableProperty] public partial string Copy { get; set; } = Configuration.StandardEnglish().Copy;
    [ObservableProperty] public partial string Help { get; set; } = Configuration.StandardEnglish().Help;

    [ObservableProperty] private string? _fileText;

    public static int TitleBarHeight => 38;
    
    [RelayCommand]
    public async Task OpenSequence()
    {
        var file = await OpenFilePickerAsync();
        Console.WriteLine("File: " + file?.Path);
    }

    [RelayCommand]
    public void ExitBeam()
    {
        Environment.Exit(0);
    }

    private async Task<IStorageFile?> OpenFilePickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        return files?.Count >= 1 ? files[0] : null;
    }
    private Logger? _logger;
    public MainWindowViewModel()
    {
        _logger = Logger.getInstance("../../../../BEAM.Tests/loggerTests/testLogs/testLog.log");
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