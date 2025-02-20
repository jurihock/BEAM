using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.ImageSequence;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEAM.Renderer;

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


    public SequenceViewModel(ISequence sequence, DockingViewModel dockingVm)
    {
        Sequence = new TransformedSequence(sequence);
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
}