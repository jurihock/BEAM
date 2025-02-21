using Avalonia.Controls;
using Avalonia.Interactivity;
using BEAM.ViewModels;

namespace BEAM.Views;

/// <summary>
/// Represents the view for exporting sequences.
/// </summary>
public partial class ExportSequenceView : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExportSequenceView"/> class.
    /// </summary>
    /// <param name="sequenceViewModel">The view model for the sequence.</param>
    public ExportSequenceView(SequenceViewModel sequenceViewModel)
    {
        DataContext = new ExportSequencePopupViewModel(sequenceViewModel);
        InitializeComponent();
    }

    /// <summary>
    /// Handles the close event to close the window.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Attempts to save the sequence and closes the window if successful.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void TrySave(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as ExportSequencePopupViewModel)!).Save())
        {
            Close();
        }
    }
}