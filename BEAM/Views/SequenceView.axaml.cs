using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using BEAM.Datatypes;
using BEAM.Image.Bitmap;
using BEAM.Image.Displayer;
using BEAM.IMage.Displayer.Scottplot;
using BEAM.Image.Minimap;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Log;
using BEAM.Profiling;
using BEAM.ViewModels;
using BEAM.Views.Minimap;
using CommunityToolkit.Mvvm.ComponentModel;
using NP.Ava.Visuals;
using ScottPlot;
using ScottPlot.Avalonia;

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

    public object? DynamicContentViewModel
    {
        get => GetValue(DynamicContentViewModelProperty);
        set => SetValue(DynamicContentViewModelProperty, value);
    }
    
    public SequenceView()
    {
        InitializeComponent();
        this.SizeChanged += OnSizeChanged;

    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var vm = DataContext as SequenceViewModel;
        if (vm == null || vm.Minimap.Count == 0)
        {
            Console.WriteLine("sth went wrong");
            return;
        }

        if (vm.Minimap[0] is MinimapPlotView minimapControl)
        {
            double newWidth = e.NewSize.Width * 0.2; // 20% of the SequenceView's width
            double newHeight = e.NewSize.Height; // Full height of the SequenceView
            
            minimapControl.AdaptSize(newWidth, newHeight);
        }
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
        using var _ = Timer.Start();
        AvaPlot1.Plot.Axes.InvertY();
        AvaPlot1.Plot.Axes.SquareUnits();

        var plottable = new BitmapPlottable(sequence);
        AvaPlot1.Plot.Add.Plottable(plottable);

        plottable.SequenceImage.RequestRefreshPlotEvent += (sender, args) => AvaPlot1.Refresh();

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
        var vm = (SequenceViewModel?)DataContext;
        if (vm == null)
        {
            return;
        }

        var menu = AvaPlot1.Menu!;
        menu.Clear();
        menu.Add("Inspect Pixel",
            control => vm.OpenInspectionViewCommand.Execute(null));
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
        if (vm == null)
        {
            return;
        }
        vm.MinimapHasGenerated += OnMinimapGenerated;
        FillPlot(vm.Sequence);
    }

    // private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    // {
    //     var point = e.GetCurrentPoint(sender as Control);
    //     var x = point.Position.X;
    //     var y = point.Position.Y;
    //
    //     Coordinates CoordInPlot = AvaPlot1.Plot.GetCoordinates(new Pixel(x, y));
    //
    //     var vm = (SequenceViewModel?)DataContext;
    //     vm.UpdateInspectionViewModel(((long)CoordInPlot.X, (long)CoordInPlot.Y));
    // }

    private void OnMinimapGenerated(object? sender, EventArgs e)
    {
        Console.WriteLine("Ready to view");
        /*Dispatcher.UIThread.InvokeAsync(() =>
        {
            var vm = DataContext as SequenceViewModel;
        });*/
    }

    private void Layoutable_OnLayoutUpdated(object? sender, EventArgs e)
    {
        var vm = DataContext as SequenceViewModel;
        if (vm == null || vm.Minimap.Count == 0)
        {
            Console.WriteLine("sth went wrong");
            return;
        }

        if (vm.Minimap[0] is MinimapPlotView minimapControl)
        {
            double newWidth = this.GetSize().X * 0.2; // 20% of the SequenceView's width
            double newHeight = this.GetSize().Y; // Full height of the SequenceView
            
            minimapControl.AdaptSize(newWidth, newHeight);
        }
    }
}