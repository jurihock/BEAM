using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ScottPlot.Avalonia;

namespace BEAM.Views;

public partial class InspectionView : UserControl
{
    public InspectionView()
    {
        InitializeComponent();
        double[] dataX = { 1, 2, 3, 2.5, 2};
        double[] dataY = { 1, 4, 9, 16, 25 };

        AvaPlot resultPlot = this.Find<AvaPlot>("AvaPlot2");
        resultPlot.Plot.Add.Bars(dataX);
        resultPlot.Plot.Axes.Margins(bottom: 0);
        resultPlot.Refresh();
    }
    
    
}