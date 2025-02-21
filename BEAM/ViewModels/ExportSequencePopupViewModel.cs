using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using BEAM.Exporter;
using BEAM.Image;
using BEAM.Models.Log;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

/// <summary>
/// ViewModel for the export sequence popup.
/// </summary>
public partial class ExportSequencePopupViewModel : ViewModelBase
{
    private readonly SequenceViewModel _sequenceViewModel;
    private IStorageFile? _folder;

    private SequenceType _selectedType;

    /// <summary>
    /// Gets or sets the selected sequence type for export.
    /// </summary>
    public SequenceType SelectedType
    {
        get => _selectedType;
        set
        {
            if (_selectedType != value)
            {
                _selectedType = value;
                OnPropertyChanged(nameof(SelectedType));
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportSequencePopupViewModel"/> class.
    /// </summary>
    /// <param name="model">The sequence view model.</param>
    public ExportSequencePopupViewModel(SequenceViewModel model)
    {
        _sequenceViewModel = model;
    }

    /// <summary>
    /// Gets the collection of available export types.
    /// </summary>
    public ObservableCollection<SequenceType> ExportTypes { get; } = new()
    {
        SequenceType.Envi,
        SequenceType.Png
    };

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public new event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected new virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Command to export the sequence.
    /// </summary>
    [RelayCommand]
    public async Task ExportSequence()
    {
        _folder = await OpenFolderPickerAsync();
    }

    /// <summary>
    /// Opens a folder picker dialog to select the export folder.
    /// </summary>
    /// <returns>The selected folder, or null if no folder was selected.</returns>
    private static async Task<IStorageFile?> OpenFolderPickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var folder = await provider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Choose sequence save folder",
        });

        return folder;
    }

    /// <summary>
    /// Saves the sequence to the selected folder.
    /// </summary>
    /// <returns>True if the save operation started successfully, otherwise false.</returns>
    public bool Save()
    {
        if (_folder == null)
        {
            return false;
        }

        switch (_selectedType)
        {
            case SequenceType.Envi:
                Task.Run(() => EnviExporter.Export(_folder, _sequenceViewModel.Sequence,
                    _sequenceViewModel.Renderers[_sequenceViewModel.RendererSelection]));
                break;
            case SequenceType.Png:
                Task.Run(() => PngExporter.Export(_folder, _sequenceViewModel.Sequence,
                    _sequenceViewModel.Renderers[_sequenceViewModel.RendererSelection]));
                break;
            default:
                return false;
        }
        Logger.GetInstance().LogMessage($"Started exporting sequence {_sequenceViewModel.Sequence.GetName()} as {_selectedType} to {_folder.Path.AbsolutePath}");
        return true;
    }
}