using Avalonia.Controls;
using BEAM.ViewModels;

namespace BEAM.Views;

/// <summary>
/// Code behind the status bar.
/// Yeah, pretty much doing nothing except creating its data context.
/// </summary>
public partial class StatusBar : UserControl
{
    public StatusBar()
    {
        InitializeComponent();
        DataContext = StatusBarViewModel.GetInstance();
    }
}