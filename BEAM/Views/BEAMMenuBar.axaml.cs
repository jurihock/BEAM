using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class BEAMMenuBar : UserControl
{
    public BEAMMenuBar()
    {
        InitializeComponent();
        DataContext = new BEAMMenuBarViewModel();
    }
}