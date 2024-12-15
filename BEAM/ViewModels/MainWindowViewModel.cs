using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using BEAM.ImageSequence;
using BEAM.Log;
using BEAM.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public static readonly Configuration BaseConfig = Configuration.StandardEnglish();
    
    [ObservableProperty] public partial string File { get; set; } = BaseConfig.FileMenu;
    [ObservableProperty] public partial string Open { get; set; } = BaseConfig.Open;
    [ObservableProperty] public partial string OpenFolder { get; set; } = BaseConfig.OpenFolder;
    [ObservableProperty] public partial string Exit { get; set; } = BaseConfig.Exit;
    [ObservableProperty] public partial string Edit { get; set; } = BaseConfig.Edit;
    [ObservableProperty] public partial string Paste { get; set; } = BaseConfig.Paste;
    [ObservableProperty] public partial string Copy { get; set; } = BaseConfig.Copy;
    [ObservableProperty] public partial string Help { get; set; } = BaseConfig.Help;

    [ObservableProperty] private string? _fileText;
    [ObservableProperty] public partial DockingViewModel DockingVm { get; set; } = new();


    public static int TitleBarHeight => 38;

    private List<Sequence> loadedSequences = [];

    private Logger? _logger;

    public MainWindowViewModel()
    {
        _logger = Logger.GetInstance();
    }
    
    [RelayCommand]
    public async Task OpenSequence()
    {
        var files = await OpenFilePickerAsync();

        if (files != null)
        {
            var list = files.Select(f => f.Path).ToList();
            try
            {
                DockingVm.OpenSequenceView(Sequence.Open(list)); 
            } catch (Exception ex) {}
        }
        else
        {
            _logger?.Warning(LogEvent.FileNotFound);
        }
    }
    
    [RelayCommand]
    public async Task OpenSequenceFromFolder()
    {
        var folder = await OpenFolderPickerAsync();

        if (folder != null)
        {
            try
            {
                loadedSequences.Add(Sequence.Open(folder.Path));
            } catch(Exception ex) {}
        }
        else
        {
            _logger?.Warning(LogEvent.FileNotFound);
        }
    }

    [RelayCommand]
    public void ExitBeam()
    {
        Environment.Exit(0);
    }

    private async Task<IStorageFolder?> OpenFolderPickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var folder = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Open Sequence Folder",
            AllowMultiple = false,
        });

        return folder?.Count >= 1 ? folder[0] : null;
    }

    private async Task<IReadOnlyList<IStorageFile>?> OpenFilePickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Text File",
            AllowMultiple = false,
        });

        return files;
    }
    
    public void AddSequence(Sequence sequence)
    {
        loadedSequences.Add(sequence);
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