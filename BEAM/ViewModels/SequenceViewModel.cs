using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Rendering;
using Avalonia.Threading;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Models.Log;
using BEAM.Views.Minimap.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace BEAM.ViewModels;

public partial class SequenceViewModel : ViewModelBase, IDockBase
{
    public EventHandler<RenderersUpdatedEventArgs> RenderersUpdated = delegate { };
    public EventHandler<RenderersUpdatedEventArgs> CutSequence = delegate { };

    public TransformedSequence Sequence { get; set; }

    public SequenceRenderer[] Renderers;
    public int RendererSelection;
    
    [ObservableProperty] public partial DockingViewModel DockingVm { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D pressedPointerPosition { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D releasedPointerPosition { get; set; } = new();
    
    private Image.Minimap.Minimap? _currentMinimap;
    public EventHandler<EventArgs> MinimapHasChanged = delegate { };


    //public ISequence Sequence { get; }

    private List<InspectionViewModel> _ConnectedInspectionViewModels = new();

    
    [ObservableProperty] public partial ObservableCollection<ViewModelBase> MinimapVms { get; set; }= new ObservableCollection<ViewModelBase>();

    public SequenceViewModel(ISequence sequence, DockingViewModel dockingVm)
    {
        Test();
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
        
        _currentMinimap = MinimapSettingsUtilityHelper.GetDefaultClones().Active;
        if (_currentMinimap is not null)
        {
            //TODO: in up to data branch the SequenceVM knows the renderer
            if(RendererSelection < Renderers.Length && RendererSelection >= 0)
            {
                _currentMinimap.SetRenderer(Renderers[RendererSelection]);
                _currentMinimap.StartGeneration(sequence, OnMinimapGenerated);
            }
        }
        
    }

    private void Test()
    {
        SettingsUtilityHelper<Image.Minimap.Minimap>.GetDefaultObjects().ForEach(Console.WriteLine);
        Console.WriteLine("------");
        SettingsUtilityHelper<IMinimapAlgorithm>.GetDefaultObjects().ForEach(Console.WriteLine);
    }

    public string Name => Sequence.GetName();

    public SequenceRenderer CurrentRenderer => Renderers[RendererSelection];
    
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
    public void UpdateInspectionViewModel(Coordinate2D point)
    {
        foreach (var inspectionViewModel in _ConnectedInspectionViewModels)
        {
            inspectionViewModel.Update(point, this);
        }
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
    public async Task OpenInspectionView()
    {
        InspectionViewModel inspectionViewModel = new InspectionViewModel(this);
        _ConnectedInspectionViewModels.Add(inspectionViewModel);
        DockingVm.OpenDock(inspectionViewModel);
        
        inspectionViewModel.Update(pressedPointerPosition, this);
    }
    

    public void OnClose()
    {
        if(_currentMinimap is not null)
        {
            _currentMinimap.StopGeneration();
        }
    }

    public override string ToString()
    {
        return Name;
    }
}