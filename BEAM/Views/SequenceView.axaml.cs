using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
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
    public SequenceView()
    {
        InitializeComponent();
    }

    private void ScottPlotTest(Sequence sequence)
    {
        var avaPlot1 = this.Find<AvaPlot>("AvaPlot1");

        if (avaPlot1 == null)
        {
            return;
        }

        _BuildCustomRightClickMenu();

        // TODO: CustomMouseActions
        // https://github.com/ScottPlot/ScottPlot/blob/main/src/ScottPlot5/ScottPlot5%20Demos/ScottPlot5%20WinForms%20Demo/Demos/CustomMouseActions.cs

        //avaPlot1.Interaction.IsEnabled = true;
        //avaPlot1.UserInputProcessor.IsEnabled = true;
        //avaPlot1.UserInputProcessor.UserActionResponses.Clear();

        //var panButton = ScottPlot.Interactivity.StandardMouseButtons.Middle;
        //var panResponse = new ScottPlot.Interactivity.UserActionResponses.MouseDragPan(panButton);
        using var _ = Timer.Start();

        // darkmode
        if (Application.Current!.ActualThemeVariant == ThemeVariant.Dark)
        {
            // change figure colors
            avaPlot1.Plot.FigureBackground.Color = Color.FromHex("#181818");
            avaPlot1.Plot.DataBackground.Color = Color.FromHex("#1f1f1f");

            // change axis and grid colors
            avaPlot1.Plot.Axes.Color(Color.FromHex("#d7d7d7"));
            avaPlot1.Plot.Grid.MajorLineColor = Color.FromHex("#404040");

            // change legend colors
            avaPlot1.Plot.Legend.BackgroundColor = Color.FromHex("#404040");
            avaPlot1.Plot.Legend.FontColor = Color.FromHex("#d7d7d7");
            avaPlot1.Plot.Legend.OutlineColor = Color.FromHex("#d7d7d7");
        }

        avaPlot1.Plot.Axes.InvertY();
        avaPlot1.Plot.Axes.SquareUnits();
        var plottable = new BitmapPlottable(sequence);
        avaPlot1.Plot.Add.Plottable(plottable);
        avaPlot1.Refresh();
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
        menu.Add("Configure colors",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
        menu.Add("Affine Transformation",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
        menu.AddSeparator();
        menu.Add("Export sequence",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
    }

    private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
    {
        var vm = DataContext as SequenceViewModel;

        ScottPlotTest(vm.Sequence);
    }
}