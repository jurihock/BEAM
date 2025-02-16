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
using BEAM.Renderer;
using CommunityToolkit.Mvvm.ComponentModel;
using NP.Utilities;

namespace BEAM.ViewModels.Minimap.Popups;

public partial class  DefaultMinimapPopupViewModel : ViewModelBase
{
    private static Control _defaultControl = new TextBox() { Text = "This Minimap provides no changeable settings" };
    
    private List<ISaveControl> _controls = [];

    private Image.Minimap.Minimap? _chosenMinimap;

    private ComboBox _minimapSelection;

    [ObservableProperty] public StackPanel minimapSubSettings = new StackPanel() { Margin = new Thickness(30, 0, 0, 0) };
    public ObservableCollection<Control> subSettings = [];

    private ISaveControl? _currentConfigControl;

   
    
    public DefaultMinimapPopupViewModel()
    {
        if (!MinimapSettingsUtilityHelper.ExistAny())
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "No Minimaps available";
            minimapSubSettings.Children.Add(textBlock);
            return;
        }
        
        _chosenMinimap = MinimapSettingsUtilityHelper.GetDefaultMinimap();
        

        for (var index = 0; index < MinimapSettingsUtilityHelper.GetDefaultMinimaps().Count(); index++)
        {
            var minimap = MinimapSettingsUtilityHelper.GetDefaultMinimaps()[index];
            _minimapSelection.Items.Add(minimap);
        }
        
        
        _minimapSelection.SelectedItem = _chosenMinimap;
        ChangeDisplayedSettings();
        
        _minimapSelection.SelectionChanged += (sender, args) =>
        {
            if(_currentConfigControl is not null)
            {
                _currentConfigControl.Save();
            }
            _chosenMinimap = (Image.Minimap.Minimap)_minimapSelection.SelectedItem;
            ChangeDisplayedSettings();
        };
    }

    private void ChangeDisplayedSettings()
    {
        if (_currentConfigControl is not null)
        {
            _currentConfigControl.Save();
        }
        minimapSubSettings.Children.Clear();
        
        //TODO: Alternatively one minimap only ever has one Control Window and we can get access to it thathw ay
        //TODO: Alternatively one ([User]Control, ISaveControl) inherits from the other
        (Control, ISaveControl)? duplicateView = (_minimapSelection.SelectedItem as Image.Minimap.Minimap)?.GetSettingsPopupControl();
        Control toDisplay;
        if (duplicateView is not null)
        {
            if(duplicateView.Value.Item1 is not null)
            {
                toDisplay = duplicateView.Value.Item1;
                _currentConfigControl = duplicateView.Value.Item2;
            }   
            else
            {
               toDisplay = _defaultControl;
               _currentConfigControl = null;
            }
        }
        else
        {
            toDisplay = new TextBox(){Text = "An Error Occured trying to get this Minimap's Control Panel;"};
            _currentConfigControl = null;
        }
        
        minimapSubSettings.Children.Add(toDisplay);
    }
    
    public bool Save()
    {
        if (_currentConfigControl is not null)
        {
            _currentConfigControl.Save();
        }
        
        MinimapSettingsUtilityHelper.SetDefaultMinimap(_chosenMinimap);
        return true;
    }
    
}

public interface ISaveControl
{
    public void Save();
}