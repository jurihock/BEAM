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
    private AvaPlot _avaPlot;
    public MinimapPlotView()
    {
        InitializeComponent();
        _avaPlot = this.Find<AvaPlot>("AvaPlot1");
        if (_avaPlot is null)
        {
            Console.WriteLine("nullllllllö");
        }
        this.DataContextChanged += DataContextChangedHandling;
    }
    public void FillPlot(Plot newPlot)
    {
        var vm = DataContext as MinimapPlotViewModel;
        _avaPlot.Reset(newPlot);
        _avaPlot.Refresh();
    }

    private void DataContextChangedHandling(object? sender, EventArgs eventArgs)
    {
        var vm = DataContext as MinimapPlotViewModel;
        vm.PropertyChanged += (s, e) => FillPlot(vm.CurrentPlot);
        FillPlot(vm.CurrentPlot);        
    }
    
}