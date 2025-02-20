using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ViewModels;

namespace BEAM.Views;

/// <summary>
/// Class representing the code behind the affine transform popup.
/// </summary>
public partial class AffineTransformationPopup : Window
{
    public AffineTransformationPopup(SequenceViewModel model)
    {
        DataContext = new AffineTransformationPopupViewModel(model);
        AddHandler(KeyDownEvent, (sender, e) =>
        {
            if (e.Key == Key.Escape) Close();
        });
        InitializeComponent();
    }

    /// <summary>
    /// Closes the popup.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Tries to save the current settings.
    /// Closes the popup if successful.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private void TrySave(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as AffineTransformationPopupViewModel)!).Save())
        {
            Close();
        }

        // If execution is here -> Save failed, hints in controls
    }
}