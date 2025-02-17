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
        Console.WriteLine("Call constructor Plot view");
    }
    public void FillPlot(Plot newPlot)
    {
        MinimapPlot.Plot.Clear();
        
        newPlot.PlotControl = MinimapPlot;
        MinimapPlot.Reset(newPlot);
        MinimapPlot.Height = 300;
        MinimapPlot.Width = 300;
        //TODO: Caused AccessViolationException sometimes?!
        MinimapPlot.Refresh();
    }
    
    private void DataContextChangedHandling(object? sender, EventArgs eventArgs)
    {
        var vm = DataContext as MinimapPlotViewModel;
        Console.WriteLine("Filling Plot of Plot view");
        FillPlot(vm.CurrentPlot); 
        
    }

    public void AdaptSize(double newWidth, double newHeight)
    {
        MinimapPlot.Width = newWidth;
        MinimapPlot.Height = newHeight;
        MinimapPlot.Refresh();
    }



}