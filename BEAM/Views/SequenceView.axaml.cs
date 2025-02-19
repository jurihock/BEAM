using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using BEAM.Datatypes;
using BEAM.IMage.Displayer.Scottplot;
using BEAM.ImageSequence;
using BEAM.Log;
using BEAM.Profiling;
using BEAM.ViewModels;
using BEAM.ViewModels.Utility;
using NP.Ava.Visuals;
using ScottPlot;
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

    public object? DynamicContentViewModel
    {
        get => GetValue(DynamicContentViewModelProperty);
        set => SetValue(DynamicContentViewModelProperty, value);
    }
    
    public SequenceView()
    {
        InitializeComponent();
        this.SizeChanged += OnSizeChanged;
        this.DataContextChanged += StyledElement_OnDataContextChanged;

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
            mapViewModel.NotifySizeChanged(this, new ViewModels.Utility.SizeChangedEventArgs(e.NewSize.Width * 0.2, e.NewSize.Height));
        }
        
    }

    private void FillPlot(Sequence sequence)
    {
        _ApplyDarkMode();
        _BuildCustomRightClickMenu();
        
        using var _ = Timer.Start();
        AvaPlot1.Plot.Axes.InvertY();
        AvaPlot1.Plot.Axes.SquareUnits();

        var plottable = new BitmapPlottable(sequence);
        AvaPlot1.Plot.Add.Plottable(plottable);

        plottable.SequenceImage.RequestRefreshPlotEvent += (_, _) => AvaPlot1.Refresh();

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
        menu.Add("Change Minimap settings for this sequence",
            control => vm.OpenMinimapSettingsCommand.Execute(null));
    }

    private void PointerPressedHandler(object sender, PointerPressedEventArgs args)
    {
        
        var point = args.GetCurrentPoint(sender as Control);
        var x = point.Position.X;
        var y = point.Position.Y;
    
        var coordInPlot = new Coordinate2D(AvaPlot1.Plot.GetCoordinates(new Pixel(x, y)));
    
        var vm = (SequenceViewModel?)DataContext;
        if (vm is null) return;
        vm.pressedPointerPosition = coordInPlot;
    }
    
    private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs args)
    {
        
        var point = args.GetCurrentPoint(sender as Control);
        var x = point.Position.X;
        var y = point.Position.Y;
    
        var CoordInPlot = new Coordinate2D(AvaPlot1.Plot.GetCoordinates(new Pixel(x, y)));
    
        var vm = (SequenceViewModel?)DataContext;
        if (vm is null) return;
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

        vm.MinimapHasChanged += Layoutable_OnLayoutUpdated;
        FillPlot(vm.Sequence);
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
            mapViewModel.NotifySizeChanged(this, new ViewModels.Utility.SizeChangedEventArgs(this.GetSize().X * 0.2, this.GetSize().Y));
        }
    }
}