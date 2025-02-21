using System;
using System.Data.Common;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Styling;
using BEAM.CustomActions;
using BEAM.Datatypes;
using BEAM.IMage.Displayer.Scottplot;
using BEAM.ImageSequence.Synchronization;
using BEAM.Models.Log;
using BEAM.ViewModels;
using BEAM.ViewModels.Utility;
using NP.Ava.Visuals;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Interactivity;
using ScottPlot.Interactivity.UserActionResponses;
using ScottPlot.Plottables;
using SizeChangedEventArgs = Avalonia.Controls.SizeChangedEventArgs;

namespace BEAM.Views;

public partial class SequenceView : UserControl
{
    // Hosts the external UserControl
    public static readonly StyledProperty<Control?> DynamicContentProperty =
        AvaloniaProperty.Register<SequenceView, Control?>(nameof(DynamicContent));
    
    // Optional: Bind to the external UserControl's ViewModel
    public static readonly StyledProperty<object?> DynamicContentViewModelProperty =
        AvaloniaProperty.Register<SequenceView, object?>(nameof(DynamicContentViewModel));
    
    public Control? DynamicContent
    {
        get => GetValue(DynamicContentProperty);
        set => SetValue(DynamicContentProperty, value);
    }
    private BitmapPlottable _plottable;
    private Annotation Anno;
    private readonly HorizontalLine _horizontalLine = new();
    private readonly VerticalLine _verticalLine = new();

    public object? DynamicContentViewModel
    {
        get => GetValue(DynamicContentViewModelProperty);
        set => SetValue(DynamicContentViewModelProperty, value);
    }
    
    public SequenceView()
    {
        InitializeComponent();
         _horizontalLine = AvaPlot1.Plot.Add.HorizontalLine(0);
        _verticalLine = AvaPlot1.Plot.Add.VerticalLine(0);
        this.SizeChanged += OnSizeChanged;
        //this.DataContextChanged += StyledElement_OnDataContextChanged;

    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var vm = DataContext as SequenceViewModel;
        if (vm == null || vm.MinimapVms.Count == 0)
        {
            return;
        }
        
        
        if(vm.MinimapVms.Any())
        {
            if(vm.MinimapVms.First() is not SizeAdjustableViewModelBase mapViewModel) return;
            mapViewModel.NotifySizeChanged(this, new ViewModels.Utility.SizeChangedEventArgs(e.NewSize.Width * MinimapWidthScale, e.NewSize.Height));
        }
    }

    private void PreparePlot()
    {
        _ApplyTheme();
        _BuildCustomRightClickMenu();

        // TODO: CustomMouseActions
        // https://github.com/ScottPlot/ScottPlot/blob/main/src/ScottPlot5/ScottPlot5%20Demos/ScottPlot5%20WinForms%20Demo/Demos/CustomMouseActions.cs

        //avaPlot1.Interaction.IsEnabled = true;
        //avaPlot1.UserInputProcessor.IsEnabled = true;
        //avaPlot1.UserInputProcessor.UserActionResponses.Clear();

        //var panButton = ScottPlot.Interactivity.StandardMouseButtons.Middle;
        //var panResponse = new ScottPlot.Interactivity.UserActionResponses.MouseDragPan(panButton);

        // Remove the standard MouseWheelZoom and replace it with the wanted custom functionality
        ScrollingSynchronizerMapper.AddSequence(this);
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
            ScrollingSynchronizerMapper.Synchronize(this);
        };

        AvaPlot1.PointerWheelChanged += (s, e) =>
        {
            UpdateScrollBar();
            ScrollingSynchronizerMapper.Synchronize(this);
        };

        AddScrollBarUpdating();

        PlotControllerManager.AddPlotToAllControllers(AvaPlot1);

        AvaPlot1.Plot.Axes.InvertY();
        AvaPlot1.Plot.Axes.SquareUnits();
        // Reset the axis to the inital value of the scrollbar.
        UpdateScrolling(0);
        AvaPlot1.Refresh();
        AvaPlot1.Plot.Add.Annotation("This is an Annotation");
    }

    private void AddScrollBarUpdating()
    {
        AvaPlot1.PointerEntered += (s, e) =>
        {
            var coordinates = AvaPlot1.Plot.GetCoordinates(new Pixel(e.GetPosition(AvaPlot1).X, e.GetPosition(AvaPlot1).Y));
            UpdatePositionAnnotation((long) coordinates.X,(long)  coordinates.Y);
            UpdateScrollBar();
        };

        AvaPlot1.PointerExited += (s, e) =>
        {
            var coordinates = AvaPlot1.Plot.GetCoordinates(new Pixel(e.GetPosition(AvaPlot1).X, e.GetPosition(AvaPlot1).Y));
            UpdatePositionAnnotation((long) coordinates.X,(long)  coordinates.Y);
            UpdateScrollBar();
        };

        AvaPlot1.PointerMoved += (s, e) =>
        {
            var coordinates = AvaPlot1.Plot.GetCoordinates(new Pixel(e.GetPosition(AvaPlot1).X, e.GetPosition(AvaPlot1).Y));
            UpdatePositionAnnotation((long) coordinates.X,(long)  coordinates.Y);
            UpdateScrollBar();
        };

        AvaPlot1.PointerPressed += (s, e) =>
        {
            var coordinates = AvaPlot1.Plot.GetCoordinates(new Pixel(e.GetPosition(AvaPlot1).X, e.GetPosition(AvaPlot1).Y));
            UpdatePositionAnnotation((long) coordinates.X,(long)  coordinates.Y);
            UpdateScrollBar();
        };

        AvaPlot1.PointerReleased += (s, e) =>
        {
            var coordinates = AvaPlot1.Plot.GetCoordinates(new Pixel(e.GetPosition(AvaPlot1).X, e.GetPosition(AvaPlot1).Y));
            UpdatePositionAnnotation((long) coordinates.X,(long)  coordinates.Y);
            UpdateScrollBar();
        };

        AvaPlot1.PointerCaptureLost += (s, e) =>
        {
            UpdateScrollBar();
        };

        AvaPlot1.PointerWheelChanged += (s, e) =>
        {
            var coordinates = AvaPlot1.Plot.GetCoordinates(new Pixel(e.GetPosition(AvaPlot1).X, e.GetPosition(AvaPlot1).Y));
            UpdatePositionAnnotation((long) coordinates.X,(long) coordinates.Y);
            UpdateScrollBar();
        };
    }

    private void _ApplyTheme()
    {
        var currentTheme = Application.Current!.ActualThemeVariant;

        // change figure colors
        Application.Current.TryGetResource("WindowBg", currentTheme, out var background);
        var backgroundColor = (Avalonia.Media.Color)background;
        AvaPlot1.Plot.FigureBackground.Color = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B);

        Application.Current.TryGetResource("BackgroundColorDark", currentTheme, out var backgroundDark);
        var backgroundColorDark = (Avalonia.Media.Color)backgroundDark;
        AvaPlot1.Plot.DataBackground.Color =
            new Color(backgroundColorDark.R, backgroundColorDark.G, backgroundColorDark.B);

        // change axis and grid colors
        Application.Current.TryGetResource("FontColorScottPlot", currentTheme, out var fontColorScottPlot);
        var fontColor = (Avalonia.Media.Color)fontColorScottPlot;
        AvaPlot1.Plot.Axes.Color(new Color(fontColor.R, fontColor.G, fontColor.B));


        // AvaPlot1.Plot.Grid.MajorLineColor = Colors.Aqua;

        // change legend colors
        // AvaPlot1.Plot.Legend.BackgroundColor = Colors.Aqua;
        // AvaPlot1.Plot.Legend.FontColor = Colors.Aqua;
        // AvaPlot1.Plot.Legend.OutlineColor = Colors.Aqua;

        // AvaPlot1.Plot.Grid.MajorLineColor = Color.FromHex("#404040");
        //
        // // change legend colors
        // AvaPlot1.Plot.Legend.BackgroundColor = Color.FromHex("#404040");
        // AvaPlot1.Plot.Legend.FontColor = new Color(fontColor.R, fontColor.G, fontColor.B);
        // AvaPlot1.Plot.Legend.OutlineColor = Color.FromHex("#d7d7d7");
    }

    private void _BuildCustomRightClickMenu()
    {
        var vm = (SequenceViewModel?)DataContext;
        if (vm is null)
        {
            return;
        }


        var menu = AvaPlot1.Menu!;
        menu.Clear();
        menu.Add("Inspect Pixel",
            _ => _OpenInspectionViewModel());
        menu.AddSeparator();
        menu.Add("Sync to this",
            _ =>
            {
                ScrollingSynchronizerMapper.ActivateSynchronization();
                ScrollingSynchronizerMapper.Synchronize(this);
                PlotControllerManager.ActivateSynchronization();
            });
        menu.AddSeparator();
        menu.Add("Configure colors", _ => _OpenColorsPopup());
        menu.Add("Affine Transformation", _ => _OpenTransformPopup());
        menu.AddSeparator();
        menu.Add("Cut Sequence", _ => _OpenCutPopup());
        menu.Add("Export sequence",
            _ => Logger.GetInstance().Warning(LogEvent.BasicMessage, "Not implemented yet!"));
        menu.Add("Change Minimap settings for this sequence",
            _ => vm.OpenMinimapSettingsCommand.Execute(null));
    }
    
    
    private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs args)
    {
        
        var point = args.GetCurrentPoint(sender as Control);
        var x = point.Position.X;
        var y = point.Position.Y;
    
        var coordInPlot = new Coordinate2D(AvaPlot1.Plot.GetCoordinates(new Pixel(x, y)));
    
        var vm = (SequenceViewModel?)DataContext;
        if (vm is null) return;
        vm.ReleasedPointerPosition = coordInPlot;
        vm.UpdateInspectionViewModel();
    }

    private void _SetPlottable(SequencePlottable plottable)
    {
         _plottable = plottable;
        if (_plottable is not null) AvaPlot1.Plot.Remove(_plottable);
        if(_plottable is null) return;
        AvaPlot1.Plot.Add.Plottable(_plottable!);
        _plottable!.SequenceImage.RequestRefreshPlotEvent += (_, _) => AvaPlot1.Refresh();
        AvaPlot1.Refresh();
    }

    private void PointerPressedHandler(object sender, PointerPressedEventArgs args)
    {

        var point = args.GetCurrentPoint(sender as Control);
        var x = point.Position.X;
        var y = point.Position.Y;

        var coordInPlot = new Coordinate2D(AvaPlot1.Plot.GetCoordinates(new Pixel(x, y)));

        var vm = (SequenceViewModel?)DataContext;
        if (vm is null) return;
        vm.PressedPointerPosition = coordInPlot;
    }

    private void PointerMovedHandler(object? sender, PointerEventArgs args)
    {

        var pointInPlot = AvaPlot1.Plot.GetCoordinates(
            (float)args.GetPosition(AvaPlot1).X * AvaPlot1.DisplayScale,
            (float)args.GetPosition(AvaPlot1).Y * AvaPlot1.DisplayScale);
        
        _horizontalLine.Position = pointInPlot.Y;
        _verticalLine.Position = pointInPlot.X;
        AvaPlot1.Refresh();
    }

    private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
    {
        var vm = DataContext as SequenceViewModel;
        if (vm == null)
        {
            return;
        }
        vm.MinimapHasChanged += Layoutable_OnLayoutUpdated;

        PreparePlot();

        var isDark = Application.Current!.ActualThemeVariant == ThemeVariant.Dark;
        var checkerBoard = new CheckerboardPlottable(isDark);
        AvaPlot1.Plot.Add.Plottable(checkerBoard);

        // sets the plottable
        _SetPlottable(new SequencePlottable(vm.Sequence, vm.CurrentRenderer));

        // Changed the sequence view -> full rerender
        vm.RenderersUpdated += (_, _) =>
        {
            if (_plottable is null) return;
            _plottable.SequenceImage.Reset();
            _plottable!.ChangeRenderer(vm.CurrentRenderer);
            AvaPlot1.Plot.MoveToTop(Anno);
            AvaPlot1.Refresh();
        };

        vm.CutSequence += (_, _) =>
        {
            _SetPlottable(new SequencePlottable(vm.Sequence, vm.CurrentRenderer));

            var oldLimits = AvaPlot1.Plot.Axes.GetLimits();
            var ySize = oldLimits.Bottom - oldLimits.Top;
            var newLimits = new AxisLimits(oldLimits.Left, oldLimits.Right, -ySize / 3, 2 * ySize / 3);
            AvaPlot1.Plot.Axes.SetLimits(newLimits);
            AvaPlot1.Plot.MoveToTop(Anno);
            AvaPlot1.Refresh();
        };
        Anno = AvaPlot1.Plot.Add.Annotation("(x: , y: ), (r: , g: , b: )");
        Anno.LabelShadowColor = Colors.Transparent;
        AvaPlot1.Plot.MoveToTop(Anno);
    }

    /// <summary>
    /// Updates the Position Annotation to show the given position and the pixel values in the plot
    /// of this position in the sequence.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate</param>
    public void UpdatePositionAnnotation(long x, long y)
    {
        if (DataContext is not SequenceViewModel vm)
        {
            return;
        }

        // If outside the sequence just show the position and no values.
        if (x >= vm.Sequence.Shape.Width || y >= vm.Sequence.Shape.Height || x < 0 || y < 0)
        {
            Anno.Text = $"(x: {x}, y: {y}, (r: , g: , b: ))";
            return;
        }

        var bytes = vm.Renderers[vm.RendererSelection].RenderPixel(vm.Sequence, x, y);
        Anno.Text = $"(x: {x}, y: {y}, (r: {bytes[3]}, g: {bytes[1]}, b: {bytes[2]})";
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
        if (DataContext is not SequenceViewModel sequenceViewModel) return;
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
    
    

    private void Layoutable_OnLayoutUpdated(object? sender, EventArgs e)
    {
        var vm = DataContext as SequenceViewModel;
        if (vm == null || vm.MinimapVms.Count == 0)
        {
            return;
        }
        
        if(vm.MinimapVms.Count > 0)
        {   
            var mapViewModel = (vm.MinimapVms.First() as SizeAdjustableViewModelBase);
            if (mapViewModel is null)
            {
                return;
            }
            mapViewModel.NotifySizeChanged(this, new ViewModels.Utility.SizeChangedEventArgs(this.GetSize().X * MinimapWidthScale, this.GetSize().Y));
        }
    }
}