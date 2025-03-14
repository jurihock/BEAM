using Avalonia.Controls;
using Avalonia.Threading;
using BEAM.ViewModels;

namespace BEAM.Views;

/// <summary>
/// The popup windwo for showing the export progress.
/// </summary>
public partial class ExportProgressWindowView : Window
{
    public ExportProgressWindowView(ExportProgressWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        vm.CloseEvent += (s, e) =>
        {
            Dispatcher.UIThread.InvokeAsync(Close);
        };
    }
    
    
}