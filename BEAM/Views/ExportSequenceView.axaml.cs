using Avalonia.Controls;
using Avalonia.Interactivity;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class ExportSequenceView : Window
{
    public ExportSequenceView(SequenceViewModel sequenceViewModel)
    {
        DataContext = new ExportSequencePopupViewModel(sequenceViewModel);
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