using Avalonia.Controls;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        DataContext = new AboutWindowViewModel();
    }
}