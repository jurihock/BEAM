using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using BEAM.CustomActions;
using BEAM.Datatypes;
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

    private Sequence _sequence;
    private int i = 0;
    public SequenceView()
    {
        InitializeComponent();
    }

    private void FillPlot(Sequence sequence)
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
        
        // Remove the standard MouseWheelZoom and replace it with the wanted custom functionality
        _sequence = sequence;
        ScrollingSynchronizer.addSequence(this);
        AvaPlot1.UserInputProcessor.RemoveAll<MouseWheelZoom>();
        AvaPlot1.UserInputProcessor.UserActionResponses.Add(new CustomMouseWheelZoom(StandardKeys.Shift,
            StandardKeys.Control));
        
        Bar1.Scroll += (s, e) =>
        {
            var plot = AvaPlot1.Plot;
            var ySize = plot.Axes.GetLimits().Bottom - plot.Axes.GetLimits().Top;
            // Minus 100 to allow to scroll higher than the sequence for a better inspection of the start.
            var top = (e.NewValue / 100.0) * sequence.Shape.Height - 100.0;
            AvaPlot1.Plot.Axes.SetLimitsY(top, top + ySize);
            AvaPlot1.Refresh();
            ScrollingSynchronizer.synchronize(this);
        };

        AvaPlot1.PointerWheelChanged += (s, e) =>
        {
            UpdateScrollBar();
            ScrollingSynchronizer.synchronize(this);
        };
        
        addScrollBarUpdating();
        
        PlotControllerManager.AddPlotToAllControllers(AvaPlot1);
        using var _ = Timer.Start();

        AvaPlot1.Plot.Axes.InvertY();
        AvaPlot1.Plot.Axes.SquareUnits();

        var plottable = new BitmapPlottable(sequence);
        AvaPlot1.Plot.Add.Plottable(plottable);

        plottable.SequenceImage.RequestRefreshPlotEvent += (sender, args) => AvaPlot1.Refresh();
        AvaPlot1.Refresh();
        
    }

    private void addScrollBarUpdating()
    {
        AvaPlot1.PointerEntered += (s, e) =>
        {
            UpdateScrollBar();
        };
        
        AvaPlot1.PointerExited += (s, e) =>
        {
            UpdateScrollBar();
        };
        
        AvaPlot1.PointerMoved += (s, e) =>
        {
            UpdateScrollBar();
        };
        
        AvaPlot1.PointerPressed += (s, e) =>
        {
            UpdateScrollBar();
        };
        
        AvaPlot1.PointerReleased += (s, e) =>
        {
            UpdateScrollBar();
        };
        
        AvaPlot1.PointerCaptureLost += (s, e) =>
        {
            UpdateScrollBar();
        };
        
        AvaPlot1.PointerWheelChanged += (s, e) =>
        {
            UpdateScrollBar();
        };
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
            control => vm.OpenInspectionViewCommand.Execute(null));
        menu.AddSeparator();
        menu.Add("Sync to this",
            control =>
            {
                ScrollingSynchronizer.activateSynchronization();
                ScrollingSynchronizer.synchronize(this);
                PlotControllerManager.activateSynchronization();
            });
        menu.AddSeparator();
        menu.Add("Configure colors",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
        menu.Add("Affine Transformation",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
        menu.AddSeparator();
        menu.Add("Export sequence",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
    }

    private void PointerPressedHandler(object sender, PointerPressedEventArgs args)
    {
        
        var point = args.GetCurrentPoint(sender as Control);
        var x = point.Position.X;
        var y = point.Position.Y;
    
        var CoordInPlot = new Coordinate2D(AvaPlot1.Plot.GetCoordinates(new Pixel(x, y)));
    
        var vm = (SequenceViewModel?)DataContext;
        vm.pressedPointerPosition = CoordInPlot;
    }
    
    private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs args)
    {
        
        var point = args.GetCurrentPoint(sender as Control);
        var x = point.Position.X;
        var y = point.Position.Y;
    
        var CoordInPlot = new Coordinate2D(AvaPlot1.Plot.GetCoordinates(new Pixel(x, y)));
    
        var vm = (SequenceViewModel?)DataContext;
        vm.releasedPointerPosition = CoordInPlot;
        vm.UpdateInspectionViewModel(CoordInPlot);
    }

    private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
    {
        var vm = DataContext as SequenceViewModel;

        FillPlot(vm.Sequence);
    }

    /// <summary>
    /// This method updates the value of the Scrollbar and display the corresponding position in the sequence.
    /// </summary>
    /// <param name="val">The new value of the ScrollBar</param>
    public void UpdateScrolling(double val)
    {
        var plot = AvaPlot1.Plot;
        var ySize = plot.Axes.GetLimits().Bottom - plot.Axes.GetLimits().Top;
        var top = (val / 100.0) * _sequence.Shape.Height - 100.0;
        AvaPlot1.Plot.Axes.SetLimitsY(top, top + ySize);
        AvaPlot1.Refresh();
        Bar1.Value = val;
    }

    /// <summary>
    /// This method updates the displayed position in the sequence to that of another AvaPlot and
    /// sets the corresponding value for the ScrollBar.
    /// </summary>
    /// <param name="otherPlot">The AvaPlot, which limits will be used to set the display limits of this sequence.</param>
    public void UpdateScrolling(AvaPlot otherPlot)
    {
        AvaPlot1.Plot.Axes.SetLimits(otherPlot.Plot.Axes.GetLimits());
        AvaPlot1.Refresh();
        UpdateScrollBar();
    }
    
    /// <summary>
    /// This method updates the value of the Scrollbar, setting it to the value corresponding to the displayed position in the sequence.
    /// </summary>
    public void UpdateScrollBar()
    {
        var val =  ((AvaPlot1.Plot.Axes.GetLimits().Top + 100.0) / _sequence.Shape.Height) * 100;
        Bar1.Value = val <= 0.0 ? 0.0 : double.Min(val, 100.0);
    }
}