using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap;
using ScottPlot;
using ScottPlot.Avalonia;

namespace BEAM.Views.Minimap;

public partial class MinimapPlotView : UserControl
{
    public MinimapPlotView()
    {
        InitializeComponent();
        this.DataContextChanged += DataContextChangedHandling;
    }
    public void FillPlot(Plot newPlot)
    {
        MinimapPlot.Plot.Clear();
        
        newPlot.PlotControl = MinimapPlot;
        MinimapPlot.Reset(newPlot);
        //TODO: Caused AccessViolationException sometimes?!
        MinimapPlot.Refresh();
    }
    
    private void DataContextChangedHandling(object? sender, EventArgs eventArgs)
    {
        var vm = DataContext as MinimapPlotViewModel;
        FillPlot(vm.CurrentPlot);        
    }


    
}