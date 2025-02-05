using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BEAM.Analysis;
using BEAM.ViewModels;
using NP.Utilities;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Statistics;

namespace BEAM.Views;

public partial class InspectionView : UserControl
{
    private int counter = 0;
    public InspectionView()
    {
        InitializeComponent();
        AnalysisPlot.Plot.Add.Bars(new double[]{1,2,3,4});
        this.DataContextChanged += (s, e) => Update();
    }
    public void Update()
    {
        Console.WriteLine("ClickUpdated Landed in View " + counter);
        var vm = DataContext as InspectionViewModel;
        //AnalysisPlot.Plot.Clear();
        //AnalysisPlot.Plot.Add.Bars(new double[]{DateTime.Now.Second});
        //AnalysisPlot.Plot.Add.Plottable(vm.CurrentPlot);
        AnalysisPlot.Reset(vm.CurrentPlot);
        AnalysisPlot.Refresh();
    }
}