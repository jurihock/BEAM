using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BEAM.Analysis;
using BEAM.ViewModels;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Statistics;

namespace BEAM.Views;

public partial class InspectionView : UserControl
{
    AvaPlot analysisPlot;
    public InspectionView()
    {
        InitializeComponent();
        analysisPlot = this.Find<AvaPlot>("AvaPlot1");
        analysisPlot.Plot.Add.Bars(new double[] { 1, 2, 3, 4, 5 });
    }

    public void Update(IPlottable newPlot)
    {
        analysisPlot.Plot.Clear();
        analysisPlot.Plot.Add.Plottable(newPlot);
        analysisPlot.Refresh();
    }



    // public void FillAnalysisView()
    // {
    //     var vm = DataContext as InspectionViewModel;
    //     vm.SetAnalysisViewCommand.Execute(null);
    // }
    
    
}