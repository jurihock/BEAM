using System.Collections.ObjectModel;
using BEAM.Docking;
using BEAM.ImageSequence;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BEAM.ViewModels;

/// <summary>
/// View model controlling the visible dock.
/// </summary>
public partial class DockingViewModel : ViewModelBase
{
    /// <summary>
    /// The visible dock items.
    /// </summary>
    public ObservableCollection<IDockBase> Items = [];

    [ObservableProperty] public partial bool HasDock { get; set; } = false;

    /// <summary>
    /// Opens a new dock window with the matching view to the model
    /// That means that the viewModel has to be placed inside the "ViewModels", and the corresponding view inside the "View" package
    /// </summary>
    /// <param name="viewModel">The ViewModel to open as a dock</param>
    public void OpenDock(IDockBase viewModel)
    {
        Items.Add(viewModel);
    }


    [RelayCommand]
    public void OpenSequenceView(ISequence sequence)
    {
        OpenDock(new SequenceViewModel(sequence, this));
        if (Items.Count > 0)
        {
            HasDock = true;
        }
    }

    /// <summary>
    /// Removed the view model from the visible dock items.
    /// </summary>
    /// <param name="viewModel"></param>
    public void RemoveDock(IDockBase viewModel)
    {
        Items.Remove(viewModel);
        if (Items.Count == 0)
        {
            HasDock = false;
        }
    }
}