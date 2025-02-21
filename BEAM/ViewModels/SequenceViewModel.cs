using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEAM.Renderer;
using BEAM.Models.Log;
using BEAM.Views.Minimap.Popups;


namespace BEAM.ViewModels;

/// <summary>
/// View model controlling the view of a single sequence.
/// Contains information about possible renderers, the selected renderer and handles redraw events.
/// </summary>
public partial class SequenceViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] public partial DockingViewModel DockingVm { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D PressedPointerPosition { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D ReleasedPointerPosition { get; set; } = new();

    private List<InspectionViewModel> _connectedInspectionViewModels = new();

    public EventHandler<RenderersUpdatedEventArgs> RenderersUpdated = delegate { };
    public EventHandler<RenderersUpdatedEventArgs> CutSequence = delegate { };

    public TransformedSequence Sequence { get; set; }

    public SequenceRenderer[] Renderers;
    public int RendererSelection;


    private Image.Minimap.Minimap? _currentMinimap;
    public EventHandler<EventArgs> MinimapHasChanged = delegate { };
    
    

    [ObservableProperty]
    public partial ObservableCollection<ViewModelBase> MinimapVms { get; set; } =
        new ObservableCollection<ViewModelBase>();

    public SequenceViewModel(ISequence sequence, DockingViewModel dockingVm)
    {
        Sequence = new TransformedSequence(sequence);
        DockingVm = dockingVm;

        DockingVm = dockingVm;

        var (min, max) = sequence switch
        {
            SkiaSequence => (0, 255),
            _ => (0, 1)
        };

        Renderers =
        [
            new ChannelMapRenderer(min, max, 2, 1, 0),
            new HeatMapRendererRB(min, max, 0, 0.1, 0.9),
            new ArgMaxRendererGrey(min, max),
            new ArgMaxRendererColorHSV(min, max)
        ];

        RendererSelection = sequence switch
        {
            SkiaSequence => 0,
            _ => 1
        };
        
        _currentMinimap = SettingsUtilityHelper<Image.Minimap.Minimap>.GetDefaultClones().Active;
        if (_currentMinimap is not null)
        {
            if (RendererSelection < Renderers.Length && RendererSelection >= 0)
            {
                _currentMinimap.SetRenderer(Renderers[RendererSelection]);
                _currentMinimap.StartGeneration(sequence, OnMinimapGenerated);
            }
        }
        
    }
    

    public void RegisterInspectionViewModel(InspectionViewModel inspectionViewModel)
    {
        _connectedInspectionViewModels.Add(inspectionViewModel);
        inspectionViewModel.Update(PressedPointerPosition.offsetBy(0.5, 0.5), ReleasedPointerPosition.offsetBy(0.5, 0.5));
    }
    
    public void UnregisterInspectionViewModel(InspectionViewModel inspectionViewModel)
    {
        _connectedInspectionViewModels.Remove(inspectionViewModel);
    }

    [RelayCommand]
    public void UpdateInspectionViewModel()
    {
        Coordinate2D pointPressed = _correctInvalid(PressedPointerPosition.offsetBy(0.5, 0.5));
        Coordinate2D pointReleased = _correctInvalid(ReleasedPointerPosition.offsetBy(0.5, 0.5));
        foreach (var inspectionViewModel in _connectedInspectionViewModels)
        {
            inspectionViewModel.Update(pointPressed, pointReleased);
        }
    }

    private Coordinate2D _correctInvalid(Coordinate2D point)
    {
        double x = point.Column;
        double y = point.Row;
        
        if(x < 0)
            x = 0;
        else if (x >= Sequence.Shape.Width)
            x = Sequence.Shape.Width - 1;
        if(y < 0)
            y = 0;
        else if(y >= Sequence.Shape.Height)
            y = Sequence.Shape.Height - 1;
        
        return new Coordinate2D(y, x);
    }

    public string Name => Sequence.GetName();

    public SequenceRenderer CurrentRenderer => Renderers[RendererSelection];

    public void Dispose()
    {
        Sequence.Dispose();
        GC.SuppressFinalize(this);
    }

    public void ChangeCurrentMinimap(Image.Minimap.Minimap minimap)
    {
        if (_currentMinimap is not null)
        {
            _currentMinimap.StopGeneration();
        }
        
        MinimapVms.Clear();
        _currentMinimap = minimap;
        _currentMinimap.SetRenderer(new ChannelMapRenderer(0, 255, 0, 1,2));
        _currentMinimap.StartGeneration(Sequence, OnMinimapGenerated);
    }

   
    
    [RelayCommand]
    public async Task OpenMinimapSettings()
    {
        DefaultMinimapPopupView minimapPopup = new(this);
        var v = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (v is null || v.MainWindow is null)
        {
            Logger.GetInstance().Error(LogEvent.Critical, "Unable to find ApplicationLifetime or MainWindow");
            return;
        }

        await minimapPopup.ShowDialog(v.MainWindow);
    }
    
    
    public void OnMinimapGenerated(object sender, MinimapGeneratedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Clear the existing minimap controls
            MinimapVms.Clear();
            
            var newMinimapVm = e.Minimap.GetViewModel();
            MinimapVms.Add(newMinimapVm);
            MinimapHasChanged(this, EventArgs.Empty);
        });
    }

    [RelayCommand]
    public void OpenInspectionView()
    {
        InspectionViewModel inspectionViewModel = new InspectionViewModel(this, DockingVm);
        _connectedInspectionViewModels.Add(inspectionViewModel);
        DockingVm.OpenDock(inspectionViewModel);
        
        inspectionViewModel.Update(PressedPointerPosition.offsetBy(0.5, 0.5), ReleasedPointerPosition.offsetBy(0.5, 0.5));
    }
    

    public void OnClose()
    {
        _currentMinimap?.StopGeneration();
    }

    public override string ToString()
    {
        return Name;
    }

    public void DisableMinimap()
    {
        _currentMinimap?.StopGeneration();
        MinimapVms.Clear();
    }
}