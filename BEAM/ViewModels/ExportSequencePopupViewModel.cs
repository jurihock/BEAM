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
using BEAM.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

public partial class ExportSequencePopupViewModel : ViewModelBase
{
    private readonly SequenceViewModel _sequenceViewModel;
    private IStorageFolder? _folder;
    
    [ObservableProperty]
    public partial string SequenceName { get; set; }
    
    private SequenceType _selectedType;

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
    
    public ExportSequencePopupViewModel(SequenceViewModel model)
    {
        _sequenceViewModel = model;
    }

    public ObservableCollection<SequenceType> ExportTypes { get; } = new()
    {
        SequenceType.Envi,
        SequenceType.Png
    };
    
    public new event PropertyChangedEventHandler PropertyChanged;

    protected new virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    [RelayCommand]
    public async Task ExportSequence()
    {
        _folder = await OpenFolderPickerAsync();
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
    
    public bool Save()
    {
        if (_folder == null)
        {
            return false;
        }

        switch (_selectedType)
        {
            case SequenceType.Envi:
                Task.Run(() => EnviExporter.Export(_folder, SequenceName, _sequenceViewModel.Sequence,
                    _sequenceViewModel.Renderers[_sequenceViewModel.RendererSelection]));
                break;
            case SequenceType.Png:
                Task.Run(() => PngExporter.Export(_folder, SequenceName, _sequenceViewModel.Sequence, 
                    _sequenceViewModel.Renderers[_sequenceViewModel.RendererSelection]));
                break;
            default:
                return false;
        }
        Logger.GetInstance().LogMessage($"Started exporting sequence {_sequenceViewModel.Sequence.GetName()} as {_selectedType} to {_folder.Path.AbsolutePath}");
        return true;
    }
}