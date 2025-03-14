using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using BEAM.ViewModels.Minimap;
using ScottPlot;

namespace BEAM.Views.Minimap;

/// <summary>
/// A user control that displays a minimap plot visualization.
/// </summary>
public partial class MinimapPlotView : UserControl
{
    public MinimapPlotView()
    {
        InitializeComponent();
        DataContextChanged += DataContextChangedHandling;
    }

    /// <summary>
    /// Configures a new plot with minimap-specific settings.
    /// </summary>
    /// <param name="newPlot">The plot to configure.</param>
    private void FillPlot(Plot newPlot)
    {
        MinimapPlot.Plot.Clear();

        newPlot.PlotControl = MinimapPlot;
        MinimapPlot.UserInputProcessor.UserActionResponses.Clear();
        MinimapPlot.Reset(newPlot);
        _ApplyTheme();
        MinimapPlot.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
        MinimapPlot.Plot.Layout.Fixed(new PixelPadding() { Left = 0, Right = 0, Top = 15, Bottom = 15 });
        MinimapPlot.Plot.Axes.Bottom.IsVisible = false;
        MinimapPlot.Height = 300;
        MinimapPlot.Width = 300;

        MinimapPlot.Refresh();
        _ApplyTheme();
    }

    /// <summary>
    /// Applies the current application theme to the plot elements.
    /// </summary>
    private void _ApplyTheme()
    {
        ThemeVariant currentTheme = Application.Current!.ActualThemeVariant;

        Application.Current.TryGetResource("MinimapAccent", currentTheme, out var accent);
        var accentMinimap = (Avalonia.Media.Color)accent!;
        Color accentMinimapColor = new Color(accentMinimap.R, accentMinimap.G, accentMinimap.B);

        foreach (var plottable in MinimapPlot.Plot.GetPlottables())
        {
            if (plottable is ScottPlot.Plottables.BarPlot bars)
            {
                // Change the color of the bar
                foreach (var barEntry in bars.Bars)
                {
                    barEntry.FillColor = accentMinimapColor;
                    barEntry.LineColor = accentMinimapColor;
                    barEntry.FillHatchColor = accentMinimapColor;
                }
            }
        }

        // change figure colors
        Application.Current.TryGetResource("WindowBg", currentTheme, out var background);
        var backgroundColor = (Avalonia.Media.Color)background!;
        MinimapPlot.Plot.FigureBackground.Color = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B);

        MinimapPlot.Plot.DataBackground.Color =
            new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B);

        // change axis and grid colors
        Application.Current.TryGetResource("BackgroundColorDark", currentTheme, out var backgroundDark);
        var backgroundColorDark = (Avalonia.Media.Color)backgroundDark!;
        MinimapPlot.Plot.Axes.Color(new Color(backgroundColorDark.R, backgroundColorDark.G, backgroundColorDark.B));

        Application.Current.TryGetResource("Separator", currentTheme, out var separatorColor);
        var lightAccentColor = (Avalonia.Media.Color)separatorColor!;
        MinimapPlot.Plot.Grid.MajorLineColor = new Color(lightAccentColor.R, lightAccentColor.G, lightAccentColor.B);
    }

    private void DataContextChangedHandling(object? sender, EventArgs eventArgs)
    {
        var vm = DataContext as MinimapPlotViewModel;
        if (vm is null)
        {
            return;
        }

        vm.NotifyOnSizeChanged(OnSizeChanged);
        vm.PropertyChanged += OnPropertyChanged;
        FillPlot(vm.CurrentPlot);
    }
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MinimapPlotViewModel.CurrentPlot)) return;
        if(DataContext is not MinimapPlotViewModel vm)
        {
            return;
        }

        FillPlot(vm.CurrentPlot);
        
    }
    private void OnSizeChanged(object sender, ViewModels.Utility.SizeChangedEventArgs e)
    {
        AdaptSize(e.Width, e.Height);
    }

    /// <summary>
    /// Handles changes to the plot size.
    /// </summary>
    private void AdaptSize(double newWidth, double newHeight)
    {
        MinimapPlot.Width = newWidth;
        MinimapPlot.Height = newHeight;
        MinimapPlot.Refresh();
    }
}