using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using BEAM.Datatypes;
using BEAM.Docking;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;
using BEAM.Views.Minimap;
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
    


    public Sequence Sequence { get; }

    private List<InspectionViewModel> _ConnectedInspectionViewModels = new();
    private List<Image.Minimap.Minimap> _minimaps;


    

    public SequenceViewModel(Sequence sequence, DockingViewModel dockingVm)
    {
        Sequence = sequence;
        DockingVm = dockingVm;
        
        var result  = MinimapSettingsUtilityHelper.GetDefaultClones();
        _minimaps = result.AllPossible.ToList();
        if (result.Active is not null)
        {
            _currentMinimap = result.Active;
            _currentMinimap.StartGeneration(sequence, OnMinimapGenerated);
        }

    }

    private void ChangeCurrentMinimap(Image.Minimap.Minimap newMinimap)
    {
        
    }
    
    private void OnMinimapChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null ||e.NewItems.Count == 0)
        {
            return;
        }
    }

    [RelayCommand]
    public async Task UpdateInspectionViewModel(Coordinate2D point)
    {
        foreach (var inspectionViewModel in _ConnectedInspectionViewModels)
        {
            inspectionViewModel.Update(point, this);
        }
    }
    
    public void OnMinimapGenerated(object sender, MinimapGeneratedEventArgs e)
    {


        Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Clear the existing minimap controls
            Minimap.Clear();

            // Get the new minimap control
            var newMinimapControl = e.Minimap.GetMinimap();
    
            // Ensure the control is not already part of another visual tree
            if (newMinimapControl.Parent is Panel parentPanel)
            {
                Console.WriteLine("Somehow has parent: " + newMinimapControl.Parent.Name);
                parentPanel.Children.Remove(newMinimapControl);
            }
            Minimap.Add(newMinimapControl);
            
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
}