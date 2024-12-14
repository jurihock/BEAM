using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class StatusBar : UserControl
{
    public StatusBar()
    {
        InitializeComponent();
        DataContext = new StatusBarViewModel();
    }
}