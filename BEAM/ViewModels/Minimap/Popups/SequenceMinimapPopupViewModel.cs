using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using BEAM.Image.Minimap;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups;

public partial class SequenceMinimapPopupViewModel : ViewModelBase
{
    
    private static readonly String _defaultControlText = "This Minimap provides no changeable settings";

    [ObservableProperty] public partial Image.Minimap.Minimap SelectedMinimap { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Image.Minimap.Minimap> minimaps { get; set; } =
        new ObservableCollection<Image.Minimap.Minimap>();
    [ObservableProperty] public partial ObservableCollection<Control> minimapSubSettings { get; set; } = new ObservableCollection<Control>();
    
    private ISaveControl _currentControl = new NullSaveConfig();
    private readonly SettingsStorer _storer;
    
    private readonly SequenceViewModel _dataBase;
    public SequenceMinimapPopupViewModel(SequenceViewModel dataBase, SettingsStorer storer)
    {
        _dataBase = dataBase;
        foreach (var entry in _dataBase.GetMinimaps())
        {
            var clone = entry.Clone();
            if (entry == _dataBase.GetCurrentMinimap())
            {
                SelectedMinimap = clone;
            }
            minimaps.Add(clone);
        }
        
        
        //var controls = SelectedMinimap.GetSettingsPopupControl(_storer);
        var controls = SelectedMinimap.GetSettingsPopupControl();
        minimapSubSettings.Clear();
        if(controls is null)
        {
            _currentControl = new NullSaveConfig();
            minimapSubSettings.Add(new TextBlock() {Text = _defaultControlText});
        }
        else
        {
            _currentControl = controls;
            minimapSubSettings.Add(controls);
        }
    }
    
    public void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        //TODO: I assume saving is not an intended feature here. Only on final close Contrary to the DefaultMinimapPopupViewModel
        //_currentControl.Save();
        Image.Minimap.Minimap? minimap;
        try
        {
            minimap = (Image.Minimap.Minimap) e.AddedItems[0]!;
        }
        catch (NullReferenceException ex)
        {
            throw new InvalidCastException("The selected Minimap is not a Minimap", ex);
        }
        
        //var controls = minimap.GetSettingsPopupControl(_storer);
        var controls = minimap.GetSettingsPopupControl();
        minimapSubSettings.Clear();
        if(controls is null)
        {
            _currentControl = new NullSaveConfig();
            minimapSubSettings.Add(new TextBlock() {Text = _defaultControlText});
        }
        else
        {
            _currentControl = controls;
            minimapSubSettings.Add(controls);
        }
    }
    
    public bool Save()
    {
        _currentControl.Save();
        _dataBase.ChangeCurrentMinimap(SelectedMinimap);   
        _dataBase.SetMinimaps(minimaps.ToList());
        
        
        return true;
    }
}