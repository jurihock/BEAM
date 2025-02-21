using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using BEAM.Image.Minimap.Utility;
using BEAM.Views.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels.Minimap.Popups;
/// <summary>
/// Handles minimap selection changes and updates the UI accordingly.
/// </summary>
public partial class DefaultMinimapPopupViewModel : ViewModelBase
{
    private const string DefaultControlText = "This Minimap provides no changeable settings";
    /// <summary>
    /// Handles minimap selection changes and updates the UI accordingly.
    /// </summary>
    [ObservableProperty] public partial Image.Minimap.Minimap? SelectedMinimap { get; set; }
    /// <summary>
    /// Gets or sets the collection of available minimaps.
    /// </summary>
    [ObservableProperty] public partial ObservableCollection<Image.Minimap.Minimap> Minimaps { get; set; } = new ObservableCollection<Image.Minimap.Minimap>();
    /// <summary>
    /// Gets or sets the collection of minimap-specific setting controls.
    /// </summary>
    [ObservableProperty] public partial ObservableCollection<Control> MinimapSubSettings { get; set; } = new ObservableCollection<Control>();


    private SaveUserControl _currentControl = new NullSaveConfig();

    private readonly SequenceViewModel _sequenceVm;


    /// <summary>
    /// Creates a new view model for the default minimap popup,
    /// asking the user to select the minimap for a specified sequence.
    /// </summary>
    /// <param name="sequenceVm">The view model of a sequence, whose setttings are being changed.</param>
    public DefaultMinimapPopupViewModel(SequenceViewModel sequenceVm)
    {
        _sequenceVm = sequenceVm;
        if (!SettingsUtilityHelper<Image.Minimap.Minimap>.ExistAny() /*!MinimapSettingsUtilityHelper.ExistAny()*/)
        {
            TextBlock textBlock = new TextBlock() { Text = "There are no Minimaps to choose from" };
            MinimapSubSettings.Add(textBlock);
            return;
        }
        foreach (var element in SettingsUtilityHelper<Image.Minimap.Minimap>.GetDefaultObjects() /*MinimapSettingsUtilityHelper.GetDefaultMinimaps()*/)
        {
            Minimaps.Add(element);
        }
        //SelectedMinimap = MinimapSettingsUtilityHelper.GetDefaultMinimap();
        SelectedMinimap = SettingsUtilityHelper<Image.Minimap.Minimap>.GetDefaultObject();

    }

    /// <summary>
    /// Handles minimap selection changes and updates the settings UI.
    /// </summary>
    /// <param name="sender">The source of the selection event.</param>
    /// <param name="e">Event data containing the selected minimap.</param>
    public void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _currentControl.Save();
        Image.Minimap.Minimap? minimap;
        try
        {
            minimap = (Image.Minimap.Minimap)e.AddedItems[0]!;
        }
        catch (NullReferenceException ex)
        {
            throw new InvalidCastException("The selected Minimap is not a Minimap", ex);
        }

        var controls = minimap.GetSettingsPopupControl();
        MinimapSubSettings.Clear();
        if (controls is null)
        {
            _currentControl = new NullSaveConfig();
            MinimapSubSettings.Add(new TextBlock() { Text = DefaultControlText });
        }
        else
        {
            _currentControl = controls;
            MinimapSubSettings.Add(controls);
        }
    }

    /// <summary>
    /// Saves the current minimap configuration.
    /// </summary>
    /// <returns>True if the save operation was successful.</returns>
    public bool Save()
    {
        _currentControl.Save();

        SettingsUtilityHelper<Image.Minimap.Minimap>.SetDefaultObject(SelectedMinimap);
        return true;
    }

    /// <summary>
    /// Applies the selected minimap configuration to the sequence.
    /// </summary>
    [RelayCommand]
    public void RenderMinimap()
    {
        if (SelectedMinimap is null)
        {
            return;
        }
        _sequenceVm.ChangeCurrentMinimap(SelectedMinimap.Clone());
    }


    /// <summary>
    /// Disables the minimap for the current sequence.
    /// </summary>
    public void DisableMinimap()
    {
        _sequenceVm.DisableMinimap();
    }
}

