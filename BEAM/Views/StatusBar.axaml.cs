using Avalonia.Controls;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class StatusBar : UserControl
{
    public StatusBar()
    {
        InitializeComponent();
        DataContext = StatusBarViewModel.GetInstance();
    }
}