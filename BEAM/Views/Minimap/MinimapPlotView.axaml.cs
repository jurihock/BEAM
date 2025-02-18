using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.ViewModels;
using BEAM.ViewModels.Minimap;
using NP.Utilities;
using ScottPlot;
using ScottPlot.Avalonia;
using SizeChangedEventArgs = Avalonia.Controls.SizeChangedEventArgs;

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
        MinimapPlot.UserInputProcessor.UserActionResponses.Clear();
        MinimapPlot.Reset(newPlot);
        MinimapPlot.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
        MinimapPlot.Plot.Layout.Fixed(new PixelPadding() {Left = 0, Right = 0, Top = 10, Bottom = 10});
        MinimapPlot.Height = 300;
        MinimapPlot.Width = 300;
        
        MinimapPlot.Refresh();
    }
    
    private void DataContextChangedHandling(object? sender, EventArgs eventArgs)
    {
        
        Console.WriteLine("Is datacontext null? " + (DataContext is null));
        Console.WriteLine(DataContext.GetType()+ " | ");
        var vm = DataContext as MinimapPlotViewModel;
        Console.WriteLine("Filling Plot of Plot view");
        vm.SizeChanged += OnSizeChanged;
        FillPlot(vm.CurrentPlot);
        

    }

    public void OnSizeChanged(object? sender, BEAM.ViewModels.Minimap.SizeChangedEventArgs e)
    {
        AdaptSize(e.Width, e.Height);
    }
    public void AdaptSize(double newWidth, double newHeight)
    {
        MinimapPlot.Width = newWidth;
        MinimapPlot.Height = newHeight;
        MinimapPlot.Refresh();
    }



}