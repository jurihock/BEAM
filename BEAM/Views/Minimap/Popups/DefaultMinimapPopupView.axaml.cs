using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BEAM.Docking;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Views.Minimap.Popups;

public partial class DefaultMinimapPopupView : Window, IDockBase
{
    public DefaultMinimapPopupView()
    {
        DataContext = new DefaultMinimapPopupViewModel();
        InitializeComponent();
    }
    
    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TrySave(object? sender, RoutedEventArgs e)
    {
        if (((DataContext as DefaultMinimapPopupViewModel)!).Save())
        {
            Close();
        }

        // If execution is here -> Save failed, hints in controls
    }

    public new string Name { get; } = "Configure Default Minimap settings";

    public void OnClose()
    {
        return;
    }
}