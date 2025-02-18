using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using BEAM.Image.Minimap;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NP.Utilities;

namespace BEAM.ViewModels.Minimap.Popups;

public partial class  DefaultMinimapPopupViewModel : ViewModelBase
{
    private static String _defaultControlText = "This Minimap provides no changeable settings";

    [ObservableProperty] public partial Image.Minimap.Minimap? SelectedMinimap { get; set; }
    [ObservableProperty] public partial ObservableCollection<Image.Minimap.Minimap> minimaps { get; set; } = new ObservableCollection<Image.Minimap.Minimap>();
    [ObservableProperty] public partial ObservableCollection<Control> minimapSubSettings { get; set; } = new ObservableCollection<Control>();
    
    private ISaveControl _currentControl = new NullSaveConfig();

    private readonly SequenceViewModel _sequenceVm;
    
    public DefaultMinimapPopupViewModel(SequenceViewModel sequenceVm)
    {
        _sequenceVm = sequenceVm;
        if (!MinimapSettingsUtilityHelper.ExistAny())
        {
            TextBlock textBlock = new TextBlock(){Text= "There are no Minimaps to choose from"};
            return;
        }
        
        
        foreach(var element in MinimapSettingsUtilityHelper.GetDefaultMinimaps())
        {
            minimaps.Add(element);
        }
        //TODO: remove before demonstration
        minimaps.Add(new PlotMinimap());
        SelectedMinimap = MinimapSettingsUtilityHelper.GetDefaultMinimap();
        
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

        //TODO: Alternatively one minimap only ever has one Control Window and we can get access to it thathw ay
        //TODO: Alternatively one ([User]Control, ISaveControl) inherits from the other
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
        
        MinimapSettingsUtilityHelper.SetDefaultMinimap(SelectedMinimap);
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

public abstract class ISaveControl : UserControl
{
    public abstract void Save();
}