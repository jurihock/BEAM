using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Log;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class StatusWindow : Window
{
    public StatusWindow()
    {
        InitializeComponent();
        DataContext = new StatusWindowViewModel();
    }
}