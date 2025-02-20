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
    [ObservableProperty] public partial Coordinate2D pressedPointerPosition { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D releasedPointerPosition { get; set; } = new();

    private List<InspectionViewModel> _ConnectedInspectionViewModels = new();

    public EventHandler<RenderersUpdatedEventArgs> RenderersUpdated = delegate { };
    public EventHandler<RenderersUpdatedEventArgs> CutSequence = delegate { };

    public TransformedSequence Sequence { get; set; }

    public SequenceRenderer[] Renderers;
    public int RendererSelection;


    private Image.Minimap.Minimap? _currentMinimap;
    public EventHandler<EventArgs> MinimapHasChanged = delegate { };


    //public ISequence Sequence { get; }


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
            new ArgMaxRendererGrey(min, max)
        ];

        RendererSelection = sequence switch
        {
            SkiaSequence => 0,
            _ => 1
        };

        //_currentMinimap = MinimapSettingsUtilityHelper.GetDefaultClones().Active;
        _currentMinimap = SettingsUtilityHelper<Image.Minimap.Minimap>.GetDefaultClones().Active;
        if (_currentMinimap is not null)
        {
            //TODO: in up to data branch the SequenceVM knows the renderer
            if (RendererSelection < Renderers.Length && RendererSelection >= 0)
            {
                _currentMinimap.SetRenderer(Renderers[RendererSelection]);
                _currentMinimap.StartGeneration(sequence, OnMinimapGenerated);
            }
        }
    }

    public void RegisterInspectionViewModel(InspectionViewModel inspectionViewModel)
    {
        _ConnectedInspectionViewModels.Add(inspectionViewModel);
        inspectionViewModel.Update(pressedPointerPosition, releasedPointerPosition);
    }

    public void UnregisterInspectionViewModel(InspectionViewModel inspectionViewModel)
    {
        _ConnectedInspectionViewModels.Remove(inspectionViewModel);
    }

    [RelayCommand]
    public void UpdateInspectionViewModel()
    {
        Coordinate2D pointPressed = _correctInvalid(pressedPointerPosition);
        Coordinate2D pointReleased = _correctInvalid(releasedPointerPosition);
        foreach (var inspectionViewModel in _ConnectedInspectionViewModels)
        {
            inspectionViewModel.Update(pointPressed, pointReleased);
        }
    }

    [RelayCommand]
    public Task OpenInspectionView()
    {
        InspectionViewModel inspectionViewModel = new InspectionViewModel(this);
        _ConnectedInspectionViewModels.Add(inspectionViewModel);
        DockingVm.OpenDock(inspectionViewModel);
        //here instead of 0, 0 the clicked position should be passed, this caused 
        //crashes sometimes, when corners were selected 
        inspectionViewModel.Update(
            new Coordinate2D(0, 0),
            new Coordinate2D(0, 0)
        );
        return Task.CompletedTask;
    }

    private Coordinate2D _correctInvalid(Coordinate2D point)
    {
        double x = point.Row;
        double y = point.Column;

        if (x < 0)
            x = 0;
        else if (x >= Sequence.Shape.Width)
            x = Sequence.Shape.Width - 1;
        if (y < 0)
            y = 0;
        else if (y >= Sequence.Shape.Height)
            y = Sequence.Shape.Height - 1;

        return new Coordinate2D(x, y);
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
        _currentMinimap.SetRenderer(new ChannelMapRenderer(0, 255, 0, 1, 2));
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

    public void OnClose()
    {
        if (_currentMinimap is not null)
        {
            _currentMinimap.StopGeneration();
        }
    }

    public override string ToString()
    {
        return Name;
    }
}