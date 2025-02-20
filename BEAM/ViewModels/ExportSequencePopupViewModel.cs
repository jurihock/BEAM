using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using BEAM.Exporter;
using BEAM.Views;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class ExportSequencePopupViewModel : ViewModelBase
{
    private readonly SequenceViewModel _sequenceViewModel;
    
    public ExportSequencePopupViewModel(SequenceViewModel model)
    {
        _sequenceViewModel = model;
    }

    [RelayCommand]
    public async Task ExportSequence()
    {
        var folder = await OpenFolderPickerAsync();
        if (folder == null) return;
        PngExporter.ExportToPng(folder.Path.ToString(), _sequenceViewModel.Sequence);
    }
    
    private static async Task<IStorageFolder?> OpenFolderPickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var folder = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Choose sequence save folder",
            AllowMultiple = false,
        });

        return folder.Count >= 1 ? folder[0] : null;
    }
}