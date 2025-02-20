using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ViewModels;

namespace BEAM.Views;

/// <summary>
/// Class being the code behind the renderer settings popup.
/// </summary>
public partial class ColorSettingsPopup : Window
{
    public ColorSettingsPopup(SequenceViewModel sequenceViewModel)
    {
        DataContext = new ColorSettingsPopupViewModel(sequenceViewModel);
        AddHandler(KeyDownEvent, (sender, e) =>
        {
            if (e.Key == Key.Escape) Close();
        });
        InitializeComponent();
    }

    /// <summary>
    /// Closes the popup.
    /// </summary>
    /// <param name="sender">Unused</param>
    /// <param name="e">Unused</param>
    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Tries to save the settings and closes the popup if successful.
    /// </summary>
    /// <param name="sender">Unused</param>
    /// <param name="e">Unused</param>
    private void TrySave(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as ColorSettingsPopupViewModel)!).Save())
        {
            Close();
        }

        // If execution is here -> Save failed, hints in controls
    }
}