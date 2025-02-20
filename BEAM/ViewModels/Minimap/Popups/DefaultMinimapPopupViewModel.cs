using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using BEAM.Image.Minimap.Utility;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels.Minimap.Popups;

public partial class  DefaultMinimapPopupViewModel : ViewModelBase
{
    private const string DefaultControlText = "This Minimap provides no changeable settings";

    [ObservableProperty] public partial Image.Minimap.Minimap? SelectedMinimap { get; set; }
    [ObservableProperty] public partial ObservableCollection<Image.Minimap.Minimap> Minimaps { get; set; } = new ObservableCollection<Image.Minimap.Minimap>();
    [ObservableProperty] public partial ObservableCollection<Control> MinimapSubSettings { get; set; } = new ObservableCollection<Control>();
    
    private SaveUserControl _currentControl = new NullSaveConfig();

    private readonly SequenceViewModel _sequenceVm;
    
    public DefaultMinimapPopupViewModel(SequenceViewModel sequenceVm)
    {
        _sequenceVm = sequenceVm;
        if (!SettingsUtilityHelper<Image.Minimap.Minimap>.ExistAny() /*!MinimapSettingsUtilityHelper.ExistAny()*/)
        {
            TextBlock textBlock = new TextBlock(){Text= "There are no Minimaps to choose from"};
            MinimapSubSettings.Add(textBlock);
            return;
        }
        foreach(var element in SettingsUtilityHelper<Image.Minimap.Minimap>.GetDefaultObjects() /*MinimapSettingsUtilityHelper.GetDefaultMinimaps()*/)
        {
            Minimaps.Add(element);
        }
        //SelectedMinimap = MinimapSettingsUtilityHelper.GetDefaultMinimap();
        SelectedMinimap = SettingsUtilityHelper<Image.Minimap.Minimap>.GetDefaultObject();
        
    }

    public void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _currentControl.Save();
        Image.Minimap.Minimap? minimap;
        try
        {
             minimap = (Image.Minimap.Minimap) e.AddedItems[0]!;
        }
        catch (NullReferenceException ex)
        {
            throw new InvalidCastException("The selected Minimap is not a Minimap", ex);
        }
        
        var controls = minimap.GetSettingsPopupControl();
        MinimapSubSettings.Clear();
        if(controls is null)
        {
            _currentControl = new NullSaveConfig();
            MinimapSubSettings.Add(new TextBlock() {Text = DefaultControlText});
        }
        else
        {
            _currentControl = controls;
            MinimapSubSettings.Add(controls);
        }
    }
    
    public bool Save()
    {
        _currentControl.Save();
        
        //MinimapSettingsUtilityHelper.SetDefaultMinimap(SelectedMinimap);
        SettingsUtilityHelper<Image.Minimap.Minimap>.SetDefaultObject(SelectedMinimap);
        return true;
    }

    [RelayCommand]
    public void RenderMinimap()
    {
        if (SelectedMinimap is null)
        {
            return;
        }
        _sequenceVm.ChangeCurrentMinimap(SelectedMinimap.Clone());
    }
    
}

