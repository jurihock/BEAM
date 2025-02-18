using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.Image.Minimap;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Log;
using BEAM.Renderer;
using BEAM.Views.Minimap;
using BEAM.Views.Minimap.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace BEAM.ViewModels;

public partial class SequenceViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] public partial DockingViewModel DockingVm { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D pressedPointerPosition { get; set; } = new();
    [ObservableProperty] public partial Coordinate2D releasedPointerPosition { get; set; } = new();
    [ObservableProperty] public partial  ObservableCollection<Control> Minimap { get; set; }= new();
    
    private Image.Minimap.Minimap _currentMinimap;
    public EventHandler<EventArgs> MinimapHasGenerated = delegate { };
    public EventHandler<EventArgs> MinimapHasChanged = delegate { };
    private readonly SettingsStorer _storer;


    public Sequence Sequence { get; }

    private List<InspectionViewModel> _ConnectedInspectionViewModels = new();
    private List<Image.Minimap.Minimap> _minimaps;


    public ImmutableList<Image.Minimap.Minimap> GetMinimaps() => _minimaps.ToImmutableList();
    public Image.Minimap.Minimap GetCurrentMinimap() => _currentMinimap;
    [ObservableProperty] public partial ObservableCollection<ViewModelBase> MinimapVms { get; set; }= new ObservableCollection<ViewModelBase>();

    public SequenceViewModel(Sequence sequence, DockingViewModel dockingVm)
    {
        Sequence = sequence;
        DockingVm = dockingVm;
        //_storer = new SettingsStorer();
        //var result  = _storer.GetDefaultMinimapClones();
        //var result  = MinimapSettingsUtilityHelper.GetDefaultClones();
        //_minimaps = result.AllPossible.ToList();
        /*if (result.Active is not null)
        {
            _currentMinimap = result.Active;
            //TODO: in up to data branch the SequenceVM knows the renderer
            _currentMinimap.SetRenderer(new ChannelMapRenderer(0, 255, 0, 1,2));
            _currentMinimap.StartGeneration(sequence, OnMinimapGenerated);
        }*/
        //TODO: Maybe Button for loading the default minimaps on sequence opening
        _currentMinimap = MinimapSettingsUtilityHelper.GetDefaultClones().Active;
        if (_currentMinimap is not null)
        {
            //TODO: in up to data branch the SequenceVM knows the renderer
            _currentMinimap.SetRenderer(new ChannelMapRenderer(0, 255, 0, 1,2));
            _currentMinimap.StartGeneration(sequence, OnMinimapGenerated);
        }

    }

    public void SetMinimaps(List<Image.Minimap.Minimap> minimaps)
    {
        _minimaps.Clear();
        foreach (var entry in minimaps)
        {
            _minimaps.Add(entry);
        }
    }
    

    
    public void ChangeCurrentMinimap(Image.Minimap.Minimap minimap)
    {
        _currentMinimap.StopGeneration();
        Minimap.Clear();
        //MinimapVms.Clear();
        _currentMinimap = minimap;
        _currentMinimap.SetRenderer(new ChannelMapRenderer(0, 255, 0, 1,2));
        _currentMinimap.StartGeneration(Sequence, OnMinimapGenerated);
    }

    [RelayCommand]
    public async Task UpdateInspectionViewModel(Coordinate2D point)
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
        Console.Write("Minimap Generated");
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Clear the existing minimap controls
            Minimap.Clear();
            MinimapVms.Clear();

            // Get the new minimap control
            //var newMinimapControl = e.Minimap.GetMinimap();
     
            // Ensure the control is not already part of another visual tree
            /*if (newMinimapControl.Parent is Panel parentPanel)
            {
                parentPanel.Children.Remove(newMinimapControl);
            }*/
            var newMinimapVm = e.Minimap.GetViewModel();
            //Minimap.Add(newMinimapControl);
            MinimapVms.Add(newMinimapVm);
            
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

    public string Name { get; } = "Eine tolle Sequence";

    public void OnClose()
    {
        _currentMinimap.StopGeneration();
    }

    public override string ToString()
    {
        return Name;
    }

    public void RenderMinimap(Image.Minimap.Minimap selectedMinimap)
    {
        throw new NotImplementedException();
    }
}