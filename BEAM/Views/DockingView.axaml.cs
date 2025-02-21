using System.Collections.Specialized;
using Avalonia.Controls;
using BEAM.Docking;
using BEAM.ViewModels;
using NP.Ava.UniDock;
using NP.Ava.UniDockService;

namespace BEAM.Views;

/// <summary>
/// Class controlling the dock library.
/// </summary>
public partial class DockingView : UserControl
{
    private readonly DockManager _dockManager;

    public DockingView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (!IsInitialized) return;
            var vm = (DockingViewModel)DataContext!;
            vm.Items.CollectionChanged += _ItemsOnCollectionChanged;
        };

        // Dock manager setup
        _dockManager = (DockManager)this.FindResource("TheDockManager")!;
        _dockManager.DockItemsViewModels = [];

        _dockManager.DockItemRemovedEvent += _OnItemRemoved;
    }

    private int _i;

    private void _ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add) return;
        if (e.NewItems is null || e.NewItems.Count == 0) return;

        var dock = (e.NewItems[0] as IDockBase)!;

        var model = new DockItemViewModelBase
        {
            DockId = $"{dock.Name}#{_i++}",
            Header = dock.Name,
            Content = dock,
            DefaultDockGroupId = "StackDock",
            IsActive = true,
            IsSelected = false,
        };
        _dockManager.DockItemsViewModels!.Add(model);
    }

    private void _OnItemRemoved(DockItemViewModelBase item)
    {
        var vm = DataContext as DockingViewModel;
        if (vm is null || (item.Content as IDockBase) is null)
        {
            return;
        }

        var dock = (item.Content as IDockBase)!;
        vm.RemoveDock(dock);
        dock.Dispose();
    }
}