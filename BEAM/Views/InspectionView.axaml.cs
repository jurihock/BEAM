using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using BEAM.Analysis;
using BEAM.ViewModels;
using NP.Utilities;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.AxisRules;
using ScottPlot.Statistics;

namespace BEAM.Views;

public partial class InspectionView : UserControl
{
    public InspectionView()
    {
        InitializeComponent();
        this.DataContextChanged += DataContextChangedHandling;
    }
    
    /// <summary>
    /// Updates the Plot with the given new plot.
    /// </summary>
    /// <param name="newPlot">The new plot to be displayed</param>
    private void FillPlot(Plot newPlot)
    {
        AnalysisPlot.Plot.Clear();
        
        newPlot.PlotControl = AnalysisPlot;
        AnalysisPlot.Reset(newPlot); //TODO: Causes Error when clicking inside the Plot
        //TODO: Caused AccessViolationException sometimes?!
        AnalysisPlot.Refresh();
    }

    private void DataContextChangedHandling(object? sender, EventArgs eventArgs)
    {
        var vm = DataContext as InspectionViewModel;
        vm.PropertyChanged += (s, e) => FillPlot(vm.CurrentPlot);
        FillPlot(vm.CurrentPlot);        
    }
    
    public void CloneButton_Clicked(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as InspectionViewModel;
        vm.Clone();
    }
    
    public void AnalysisPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = DataContext as InspectionViewModel;
        vm.ChangeAnalysis(AnalysisPicker.SelectedIndex);
        AnalysisPlot.Refresh();
    }
    
    public void SequencePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = DataContext as InspectionViewModel;
        vm.ChangeSequence(SequencePicker.SelectedIndex);
    }
}