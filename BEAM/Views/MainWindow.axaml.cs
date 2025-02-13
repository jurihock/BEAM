using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ImageSequence;
using BEAM.Models;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContextChanged += (sender, args) =>
        {
            if (!IsInitialized) return;
            var viewmodel = (MainWindowViewModel)DataContext!;
            DockView.DataContext = viewmodel.DockingVm;
        };

        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        var vm = (MainWindowViewModel)DataContext!;

        var data = e.Data.GetFiles();
        if (data is null) return;

        var list = data.Select(f => f.Path).ToList();
        vm.DockingVm.OpenSequenceView(DiskSequence.Open(list));
    }
}