using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public InspectionView()
    {
        InitializeComponent();
    }

    public void Update(IPlottable newPlot)
    {
        Console.WriteLine("ClickUpdated Landed in View");
        analysisPlot.Plot.Clear();
        //analysisPlot.Plot.Add.Plottable(newPlot);
        analysisPlot.Plot.Add.Bars(new double[] { 4, 4, 4, 4 });
        analysisPlot.Refresh();
    }



    // public void FillAnalysisView()
    // {
    //     var vm = DataContext as InspectionViewModel;
    //     vm.SetAnalysisViewCommand.Execute(null);
    // }
    
    
}