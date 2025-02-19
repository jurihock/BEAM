using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using BEAM.ViewModels.Minimap;
using ScottPlot;

namespace BEAM.Views.Minimap;

public partial class MinimapPlotView : UserControl
{
    private bool _isDarkMode;
    public MinimapPlotView()
    {
        InitializeComponent();
        this.DataContextChanged += DataContextChangedHandling;
    }
    private void FillPlot(Plot newPlot)
    {
        MinimapPlot.Plot.Clear();
        
        newPlot.PlotControl = MinimapPlot;
        MinimapPlot.UserInputProcessor.UserActionResponses.Clear();
        MinimapPlot.Reset(newPlot);
        _ApplyDarkMode();
        MinimapPlot.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
        MinimapPlot.Plot.Layout.Fixed(new PixelPadding() {Left = 0, Right = 0, Top = 15, Bottom = 15});
        MinimapPlot.Plot.Axes.Bottom.IsVisible = false;
        MinimapPlot.Height = 300;
        MinimapPlot.Width = 300;
        
        MinimapPlot.Refresh();
        _ApplyDarkMode();
    }
    
    private void _ApplyDarkMode()
    {
        if ((Application.Current!.ActualThemeVariant != ThemeVariant.Dark) || _isDarkMode) return;
        foreach (var plottable in MinimapPlot.Plot.GetPlottables())
        {
            if (plottable is ScottPlot.Plottables.BarPlot bars)
            {
                
                // Change the color of the bar
                foreach (var barEntry in bars.Bars)
                {
                    barEntry.FillColor = Color.FromHex("#ff6f00");
                    barEntry.LineColor = Color.FromHex("#ff6f00");
                    barEntry.FillHatchColor = Color.FromHex("#ff6f00");
                }
            }
        }
        // change figure colors
        MinimapPlot.Plot.FigureBackground.Color = Color.FromHex("#181818");
        MinimapPlot.Plot.DataBackground.Color = Color.FromHex("#1f1f1f");

        // change axis and grid colors
        MinimapPlot.Plot.Axes.Color(Color.FromHex("#d7d7d7"));
        MinimapPlot.Plot.Grid.MajorLineColor = Color.FromHex("#404040");

        // change legend colors
        MinimapPlot.Plot.Legend.BackgroundColor = Color.FromHex("#404040");
        MinimapPlot.Plot.Legend.FontColor = Color.FromHex("#d7d7d7");
        MinimapPlot.Plot.Legend.OutlineColor = Color.FromHex("#d7d7d7");
        _isDarkMode = true;
    }
    
    private void DataContextChangedHandling(object? sender, EventArgs eventArgs)
    {
        
        var vm = DataContext as MinimapPlotViewModel;
        if (vm is null)
        {
            return;
        }
        
        vm.NotifyOnSizeChanged(OnSizeChanged);
        FillPlot(vm.CurrentPlot);
        

    }

    private void OnSizeChanged(object sender, ViewModels.Utility.SizeChangedEventArgs e)
    {
        AdaptSize(e.Width, e.Height);
    }
    private void AdaptSize(double newWidth, double newHeight)
    {
        MinimapPlot.Width = newWidth;
        MinimapPlot.Height = newHeight;
        MinimapPlot.Refresh();
    }



}