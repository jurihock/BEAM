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
    
    
    private void FillPlot(Plot newPlot)
    {
        AnalysisPlot.Plot.Clear();
        
        //TODO: Copy Rules from given Plot to new Plot (Does not work yet)
        AnalysisPlot.Plot.Axes.SetLimits(newPlot.Axes.GetLimits());
        AnalysisPlot.Plot.Axes.Rules.Clear();
        for(int i = 0; i < newPlot.Axes.Rules.Count; i++)
        {
            AnalysisPlot.Plot.Axes.Rules.Add(newPlot.Axes.Rules[i]);
        }
        
        AnalysisPlot.Plot.Add.Plottable(newPlot.PlottableList[0]);
        //AnalysisPlot.Reset(newPlot); //TODO: Causes Error when clicking inside the Plot?
        //TODO: Caused AccessViolationException one?!
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