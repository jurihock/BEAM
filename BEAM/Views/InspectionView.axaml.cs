using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
        if (DataContext is not InspectionViewModel vm || vm.CurrentPlot is null) return;
        vm.PropertyChanged += (_, _) => FillPlot(vm.CurrentPlot);
        FillPlot(vm.CurrentPlot);
        SequencePicker.SelectedIndex = 0;
        AnalysisPicker.SelectedIndex = 0;
    }
    
    public void CloneButton_Clicked(object sender, RoutedEventArgs e)
    {
        if(DataContext is not InspectionViewModel vm) return;
        vm.Clone();
    }
    
    public void AnalysisPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not InspectionViewModel vm || vm.CurrentPlot is null) return;
        vm.ChangeAnalysis(AnalysisPicker.SelectedIndex);
        FillPlot(vm.CurrentPlot);
    }
    
    public void SequencePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not InspectionViewModel vm) return;
        vm.ChangeSequence(SequencePicker.SelectedIndex);
    }
    
    public void CheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is not InspectionViewModel vm) return;
        vm.CheckBoxChanged(KeepDataCheckBox.IsChecked);
    }
}