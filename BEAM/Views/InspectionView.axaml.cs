using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BEAM.Docking;
using BEAM.ViewModels;
using ScottPlot;

namespace BEAM.Views;

/// <summary>
/// Code behind the inspection view.
/// </summary>
// ReSharper disable once UnusedType.Global
public partial class InspectionView : UserControl
{
    public InspectionView()
    {
        InitializeComponent();
        DataContextChanged += DataContextChangedHandling;
    }
    
    /// <summary>
    /// Updates the Plot with the given new plot.
    /// </summary>
    /// <param name="newPlot">The new plot to be displayed</param>
    private void FillPlot(Plot newPlot)
    {
        newPlot.PlotControl = AnalysisPlot;
        AnalysisPlot.Reset(newPlot);
        AnalysisPlot.Refresh();
    }

    private void DataContextChangedHandling(object? sender, EventArgs eventArgs)
    {
        var vm = DataContext as InspectionViewModel;
        vm.PropertyChanged += (s, e) => FillPlot(vm.CurrentPlot);
        FillPlot(vm.CurrentPlot);
        SequencePicker.SelectedIndex = 0;
        AnalysisPicker.SelectedIndex = 0;
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
        FillPlot(vm.CurrentPlot);
    }
    
    public void SequencePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = DataContext as InspectionViewModel;
        vm.ChangeSequence(SequencePicker.SelectedIndex);
    }
    
    public void CheckBox_Changed(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as InspectionViewModel;
        vm.CheckBoxChanged(KeepDataCheckBox.IsChecked);
    }
}