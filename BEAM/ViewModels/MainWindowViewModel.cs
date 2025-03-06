using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.ImageSequence.Synchronization;
using BEAM.ImageSequence.Synchronization.Manipulators;
using BEAM.Models.Log;
using BEAM.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

/// <summary>
/// View model controlling the main window.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public partial string? FileText { get; set; }

    /// <summary>
    /// The view model for the docking manager.
    /// Used for opening new docks.
    /// </summary>
    [ObservableProperty]
    public partial DockingViewModel DockingVm { get; set; } = new();

    /// <summary>
    /// The height of the title bar.
    /// </summary>
    public static int TitleBarHeight => 38;

    /// <summary>
    /// The controller for handling the synchronization of the open plots.
    /// </summary>
    private SyncedPlotController? _syncedPlotController;

    /// <summary>
    /// Constructor for the main window view model.
    ///
    /// Initializes the synced plot controller for
    /// handling the synchronization of the open plots.
    /// </summary>
    public MainWindowViewModel()
    {
        _syncedPlotController = new SyncedPlotController();
        _syncedPlotController.Register(new MouseManipulator());
        PlotControllerManager.RegisterController(_syncedPlotController);
    }

    /// <summary>
    /// Opens the file picker for a single file
    /// and tries to open the selected file as a sequence.
    /// </summary>
    [RelayCommand]
    private async Task OpenSequence()
    {
        var files = await OpenFilePickerAsync();
        if (files == null || files.Count == 0) return;

        var list = files.Select(f => f.Path).ToList();

        try
        {
            DockingVm.OpenSequenceView(DiskSequence.Open(list));
        }
        catch (UnknownSequenceException)
        {
            Logger.GetInstance().Error(LogEvent.OpenedFile,
                $"Cannot open files since no suitable sequence type found. (Supported sequences: {string.Join(", ", DiskSequence.SupportedSequences)})");
        }
        catch (EmptySequenceException)
        {
            Logger.GetInstance().Info(LogEvent.OpenedFile, "The sequence to be opened is empty");
        }
        catch (InvalidSequenceException)
        {
            // not handled, since the sequence will write a log message
        }
    }

    /// <summary>
    /// Opens the folder picker and
    /// tries to open the selected folder as a sequence.
    /// </summary>
    [RelayCommand]
    private async Task OpenSequenceFromFolder()
    {
        var folder = await OpenFolderPickerAsync();

        if (folder == null) return;

        try
        {
            DockingVm.OpenSequenceView(DiskSequence.Open(folder.Path));
        }
        catch (UnknownSequenceException)
        {
            Logger.GetInstance().Error(LogEvent.OpenedFile,
                $"Cannot open folder since no suitable sequence type found. (Supported sequences: {string.Join(", ", DiskSequence.SupportedSequences)})");
        }
        catch (EmptySequenceException)
        {
            Logger.GetInstance().Info(LogEvent.OpenedFile, "The sequence to be opened is empty");
        }
        catch (InvalidSequenceException)
        {
            // not handled, since the sequence will write a log message
        }
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    [RelayCommand]
    private void ExitBeam()
    {
        Environment.Exit(0);
    }

    /// <summary>
    /// Opens the folder picker and returns the Folder.
    /// </summary>
    private static async Task<IStorageFolder?> OpenFolderPickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var folder = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Open Sequence Folder",
            AllowMultiple = false,
        });

        return folder.Count >= 1 ? folder[0] : null;
    }

    /// <summary>
    /// Opens the file picker and returns the Files.
    /// </summary>
    private static async Task<IReadOnlyList<IStorageFile>?> OpenFilePickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Sequence File(s)",
            AllowMultiple = true,
        });

        return files;
    }

    /// <summary>
    /// Opens the status window.
    /// </summary>
    [RelayCommand]
    private void OpenStatusWindow()
    {
        var statusWindow = new StatusWindow();
        statusWindow.Show();
    }

    /// <summary>
    /// Opens the about window.
    /// </summary> 
    [RelayCommand]
    private void OpenAboutWindow()
    {
        var aboutWindow = new AboutWindow();
        var v = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (v is null || v.MainWindow is null)
        {
            Logger.GetInstance().Error(LogEvent.Critical, "Unable to find ApplicationLifetime or MainWindow");
            return;
        }

        aboutWindow.ShowDialog(v.MainWindow);
    }

    /// <summary>
    /// starts the synchronization of the plots. (without resizing)
    /// </summary>
    [RelayCommand]
    private void ActivateSynchronization()
    {
        _syncedPlotController?.Activate();
        ScrollingSynchronizerMapper.ActivateSynchronization();
    }

    /// <summary>
    /// stops the synchronization of the plots.
    /// </summary>
    [RelayCommand]
    private void DeactivateSynchronization()
    {
        _syncedPlotController?.Deactivate();
        ScrollingSynchronizerMapper.DeactivateSynchronization();
    }
}