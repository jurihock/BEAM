using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class ColorSettingsPopup : Window
{
    public ColorSettingsPopup(SequenceViewModel sequenceViewModel)
    {
        DataContext = new ColorSettingsPopupViewModel(sequenceViewModel);
        AddHandler(KeyDownEvent, (_, e) =>
        {
            if (e.Key == Key.Escape) Close();
        });
        InitializeComponent();
    }

    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TrySave(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as ColorSettingsPopupViewModel)!).Save())
        {
            Close();
        }

        // If execution is here -> Save failed, hints in controls
    }
}