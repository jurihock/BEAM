using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ViewModels;

namespace BEAM.Views;

/// <summary>
/// Code behind the cut sequence popup.
/// </summary>
public partial class CutSequencePopup : Window
{
    public CutSequencePopup(SequenceViewModel sequenceViewModel)
    {
        DataContext = new CutSequencePopupViewModel(sequenceViewModel);
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
    /// Tries to save the current settings and closes if successful.
    /// </summary>
    /// <param name="sender">Unused</param>
    /// <param name="e">Unused</param>
    private void TrySave(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as CutSequencePopupViewModel)!).Save())
        {
            Close();
        }

        // If execution is here -> Save failed, hints in controls
    }
}