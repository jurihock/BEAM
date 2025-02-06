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
using ScottPlot.Statistics;

namespace BEAM.Views;

public partial class InspectionView : UserControl
{
    private int counter = 0;
    public InspectionView()
    {
        InitializeComponent();
        this.DataContextChanged += DataContextChangedHandling;
    }
    public void FillPlot(Plot newPlot)
    {
        var vm = DataContext as InspectionViewModel;
        AnalysisPlot.Reset(newPlot);
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