using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.ViewModels;
using ScottPlot.Avalonia;

namespace BEAM.Views;

public partial class InspectionView : UserControl
{
    public InspectionView()
    {
        InitializeComponent();
    }

    // public void FillAnalysisView()
    // {
    //     var vm = DataContext as InspectionViewModel;
    //     vm.SetAnalysisViewCommand.Execute(null);
    // }
    
    
}