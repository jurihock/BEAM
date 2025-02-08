using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using BEAM.Image.Bitmap;
using BEAM.Image.Displayer;
using BEAM.IMage.Displayer.Scottplot;
using BEAM.ImageSequence;
using BEAM.Log;
using BEAM.Profiling;
using BEAM.ViewModels;
using ScottPlot;
using ScottPlot.Avalonia;

namespace BEAM.Views;

public partial class SequenceView : UserControl
{
    private BitmapPlottable _plottable;

    public SequenceView()
    {
        InitializeComponent();
    }

    private void PreparePlot()
    {
        _ApplyDarkMode();
        _BuildCustomRightClickMenu();

        // TODO: CustomMouseActions
        // https://github.com/ScottPlot/ScottPlot/blob/main/src/ScottPlot5/ScottPlot5%20Demos/ScottPlot5%20WinForms%20Demo/Demos/CustomMouseActions.cs

        //avaPlot1.Interaction.IsEnabled = true;
        //avaPlot1.UserInputProcessor.IsEnabled = true;
        //avaPlot1.UserInputProcessor.UserActionResponses.Clear();

        //var panButton = ScottPlot.Interactivity.StandardMouseButtons.Middle;
        //var panResponse = new ScottPlot.Interactivity.UserActionResponses.MouseDragPan(panButton);
        AvaPlot1.Plot.Axes.InvertY();
        AvaPlot1.Plot.Axes.SquareUnits();
        AvaPlot1.Refresh();
    }

    private void _ApplyDarkMode()
    {
        if (Application.Current!.ActualThemeVariant != ThemeVariant.Dark) return;

        // change figure colors
        AvaPlot1.Plot.FigureBackground.Color = Color.FromHex("#181818");
        AvaPlot1.Plot.DataBackground.Color = Color.FromHex("#1f1f1f");

        // change axis and grid colors
        AvaPlot1.Plot.Axes.Color(Color.FromHex("#d7d7d7"));
        AvaPlot1.Plot.Grid.MajorLineColor = Color.FromHex("#404040");

        // change legend colors
        AvaPlot1.Plot.Legend.BackgroundColor = Color.FromHex("#404040");
        AvaPlot1.Plot.Legend.FontColor = Color.FromHex("#d7d7d7");
        AvaPlot1.Plot.Legend.OutlineColor = Color.FromHex("#d7d7d7");
    }

    private void _BuildCustomRightClickMenu()
    {
        var menu = AvaPlot1.Menu!;
        menu.Clear();
        menu.Add("Inspect Pixel",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
        menu.AddSeparator();
        menu.Add("Sync to this",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
        menu.AddSeparator();
        menu.Add("Configure colors", control => _OpenColorsPopup());
        menu.Add("Affine Transformation", control => _OpenTransformPopup());
        menu.AddSeparator();
        menu.Add("Cut Sequence",  control => _OpenCutPopup());
        menu.Add("Export sequence",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
    }

    private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
    {
        var vm = DataContext as SequenceViewModel;

        PreparePlot();

        _plottable = new BitmapPlottable(vm.Sequence, vm.CurrentRenderer);
        AvaPlot1.Plot.Add.Plottable(_plottable);

        _plottable.SequenceImage.RequestRefreshPlotEvent += (sender, args) => AvaPlot1.Refresh();

        AvaPlot1.Refresh();

        // Changed the sequence view -> full rerender
        vm.RenderersUpdated += (_, args) =>
        {
            _plottable.SequenceImage.Reset();
            _plottable.ChangeRenderer(vm.CurrentRenderer);
            AvaPlot1.Refresh();
        };
        
        vm.CutSequence += (_, args) =>
        {
            AvaPlot1.Plot.Remove(_plottable);
            _plottable = new BitmapPlottable(vm.Sequence, vm.CurrentRenderer);
            AvaPlot1.Plot.Add.Plottable(_plottable);
            _plottable.SequenceImage.Reset();
            var oldLimits = AvaPlot1.Plot.Axes.GetLimits();
            var ySize = oldLimits.Bottom - oldLimits.Top;
            var newLimits = new AxisLimits(oldLimits.Left, oldLimits.Right, -ySize / 3, 2 * ySize /  3);
            AvaPlot1.Plot.Axes.SetLimits(newLimits);
            AvaPlot1.Refresh();
        };
    }

    private void _OpenTransformPopup()
    {
        AffineTransformationPopup popup = new(DataContext as SequenceViewModel);
        var v = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        popup.ShowDialog(v.MainWindow);
    }

    private void _OpenColorsPopup()
    {
        ColorSettingsPopup popup = new(DataContext as SequenceViewModel);
        var v = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        popup.ShowDialog(v.MainWindow);
    }
    
    private void _OpenCutPopup()
    {
        CutSequencePopup popup = new(DataContext as SequenceViewModel);
        var v = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        popup.ShowDialog(v.MainWindow);
    }
}