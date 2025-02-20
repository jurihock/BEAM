using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class CutSequencePopup : Window
{
    public CutSequencePopup(SequenceViewModel sequenceViewModel)
    {
        DataContext = new CutSequencePopupViewModel(sequenceViewModel);
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
        if (((DataContext as CutSequencePopupViewModel)!).Save())
        {
            Close();
        }

        // If execution is here -> Save failed, hints in controls
    }
}