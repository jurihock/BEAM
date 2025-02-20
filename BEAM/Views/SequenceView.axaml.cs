using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Styling;
using BEAM.CustomActions;
using BEAM.Datatypes;
using BEAM.Image.Displayer.Scottplot;
using BEAM.Image.Displayer.ScottPlot;
using BEAM.ImageSequence.Synchronization;
using BEAM.Models.Log;
using BEAM.ViewModels;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Interactivity;
using ScottPlot.Interactivity.UserActionResponses;
using ScottPlot.Plottables;

namespace BEAM.Views;

public partial class SequenceView : UserControl
{
    private SequencePlottable _plottable;
    private HorizontalLine _horizontalLine;
    private VerticalLine _verticalLine;

    public SequenceView()
    {
        InitializeComponent();
        _horizontalLine = AvaPlot1.Plot.Add.HorizontalLine(0);
        _verticalLine = AvaPlot1.Plot.Add.VerticalLine(0);
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

        // Remove the standard MouseWheelZoom and replace it with the wanted custom functionality
        ScrollingSynchronizer.addSequence(this);
        AvaPlot1.UserInputProcessor.RemoveAll<MouseWheelZoom>();
        AvaPlot1.UserInputProcessor.RemoveAll<MouseDragZoom>(); // Remove option to zoom with right key
        AvaPlot1.UserInputProcessor.UserActionResponses.Add(new CustomMouseWheelZoom(StandardKeys.Shift,
            StandardKeys.Control));

        // Add ability to select area with right mouse button pressed
        AvaPlot1.UserInputProcessor.UserActionResponses.Add(new CustomAreaSelection(StandardMouseButtons.Right));

        Bar1.Scroll += (s, e) =>
        {
            var vm = (DataContext as SequenceViewModel)!;
            var plot = AvaPlot1.Plot;
            var ySize = plot.Axes.GetLimits().Bottom - plot.Axes.GetLimits().Top;
            // Minus 100 to allow to scroll higher than the sequence for a better inspection of the start.
            var top = (e.NewValue / 100.0) * vm.Sequence.Shape.Height - 100.0;
            AvaPlot1.Plot.Axes.SetLimitsY(top, top + ySize);
            AvaPlot1.Refresh();
            ScrollingSynchronizer.synchronize(this);
        };

        AvaPlot1.PointerWheelChanged += (s, e) =>
        {
            UpdateScrollBar();
            ScrollingSynchronizer.synchronize(this);
        };

        AddScrollBarUpdating();

        PlotControllerManager.AddPlotToAllControllers(AvaPlot1);

        AvaPlot1.Plot.Axes.InvertY();
        AvaPlot1.Plot.Axes.SquareUnits();
        AvaPlot1.Refresh();
    }

    private void AddScrollBarUpdating()
    {
        AvaPlot1.PointerEntered += (s, e) => { UpdateScrollBar(); };

        AvaPlot1.PointerExited += (s, e) => { UpdateScrollBar(); };

        AvaPlot1.PointerMoved += (s, e) => { UpdateScrollBar(); };

        AvaPlot1.PointerPressed += (s, e) => { UpdateScrollBar(); };

        AvaPlot1.PointerReleased += (s, e) => { UpdateScrollBar(); };

        AvaPlot1.PointerCaptureLost += (s, e) => { UpdateScrollBar(); };

        AvaPlot1.PointerWheelChanged += (s, e) => { UpdateScrollBar(); };
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
        var vm = DataContext as SequenceViewModel;
        var menu = AvaPlot1.Menu!;
        menu.Clear();
        menu.Add("Inspect Pixel",
            control => _OpenInspectionViewModel());
        menu.AddSeparator();
        menu.Add("Sync to this",
            control =>
            {
                ScrollingSynchronizer.activateSynchronization();
                ScrollingSynchronizer.synchronize(this);
                PlotControllerManager.activateSynchronization();
            });
        menu.AddSeparator();
        menu.Add("Configure colors", control => _OpenColorsPopup());
        menu.Add("Affine Transformation", control => _OpenTransformPopup());
        menu.AddSeparator();
        menu.Add("Cut Sequence", control => _OpenCutPopup());
        menu.Add("Export sequence",
            control => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
    }

    private void _SetPlottable(SequencePlottable plottable)
    {
        if (_plottable is not null) AvaPlot1.Plot.Remove(_plottable);

        _plottable = plottable;
        AvaPlot1.Plot.Add.Plottable(_plottable);
        _plottable.SequenceImage.RequestRefreshPlotEvent += (sender, args) => AvaPlot1.Refresh();
        AvaPlot1.Refresh();
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
        vm.UpdateInspectionViewModel();
    }

    private void PointerMovedHandler(object? sender, PointerEventArgs args)
    {
        var point = args.GetCurrentPoint(sender as Control);
        var x = point.Position.X;
        var y = point.Position.Y;

        var pointInPlot = AvaPlot1.Plot.GetCoordinates(
            (float)args.GetPosition(AvaPlot1).X * AvaPlot1.DisplayScale,
            (float)args.GetPosition(AvaPlot1).Y * AvaPlot1.DisplayScale);

        _horizontalLine.Position = pointInPlot.Y;
        _verticalLine.Position = pointInPlot.X;
        AvaPlot1.Refresh();
    }

    private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
    {
        var vm = (SequenceViewModel)DataContext!;

        PreparePlot();

        var isDark = Application.Current!.ActualThemeVariant == ThemeVariant.Dark;
        var checkerBoard = new CheckerboardPlottable(isDark);
        AvaPlot1.Plot.Add.Plottable(checkerBoard);

        // sets the plottable
        _SetPlottable(new SequencePlottable(vm.Sequence, vm.CurrentRenderer));

        // Changed the sequence view -> full rerender
        vm.RenderersUpdated += (_, args) =>
        {
            _plottable!.SequenceImage.Reset();
            _plottable!.ChangeRenderer(vm.CurrentRenderer);
            AvaPlot1.Refresh();
        };

        vm.CutSequence += (_, args) =>
        {
            _SetPlottable(new SequencePlottable(vm.Sequence, vm.CurrentRenderer));

            var oldLimits = AvaPlot1.Plot.Axes.GetLimits();
            var ySize = oldLimits.Bottom - oldLimits.Top;
            var newLimits = new AxisLimits(oldLimits.Left, oldLimits.Right, -ySize / 3, 2 * ySize / 3);
            AvaPlot1.Plot.Axes.SetLimits(newLimits);
            AvaPlot1.Refresh();
        };
    }

    private void _OpenTransformPopup()
    {
        AffineTransformationPopup popup = new((SequenceViewModel)DataContext!);
        var v = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        popup.ShowDialog(v!.MainWindow!);
    }

    private void _OpenColorsPopup()
    {
        ColorSettingsPopup popup = new((SequenceViewModel)DataContext!);
        var v = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        popup.ShowDialog(v!.MainWindow!);
    }

    private void _OpenCutPopup()
    {
        CutSequencePopup popup = new((SequenceViewModel)DataContext!);
        var v = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        popup.ShowDialog(v!.MainWindow!);
    }

    private void _OpenInspectionViewModel()
    {
        SequenceViewModel sequenceViewModel = DataContext as SequenceViewModel;
        sequenceViewModel.OpenInspectionView();
    }

    /// <summary>
    /// This method updates the value of the Scrollbar and display the corresponding position in the sequence.
    /// </summary>
    /// <param name="val">The new value of the ScrollBar</param>
    public void UpdateScrolling(double val)
    {
        var vm = (DataContext as SequenceViewModel)!;
        var plot = AvaPlot1.Plot;
        var ySize = plot.Axes.GetLimits().Bottom - plot.Axes.GetLimits().Top;
        var top = (val / 100.0) * vm.Sequence.Shape.Height - 100.0;
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
        var vm = (DataContext as SequenceViewModel)!;
        var val = ((AvaPlot1.Plot.Axes.GetLimits().Top + 100.0) / vm.Sequence.Shape.Height) * 100;
        Bar1.Value = val <= 0.0 ? 0.0 : double.Min(val, 100.0);
    }
}