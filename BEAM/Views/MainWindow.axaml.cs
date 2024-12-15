using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BEAM.Models;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class MainWindow : Window
{
    private DockingViewModel _dockingViewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DockView.DataContext = _dockingViewModel;
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private static void OnDrop(object? sender, DragEventArgs e)
    {
        Console.WriteLine("Dropped");
        var data = e.Data.GetFiles();
        var viewmodel = (MainWindowViewModel) ((MainWindow) sender!).DataContext;
        List<string> list = new();
        foreach (var file in data)
        {
            var path = file.Path.ToString();
            if (Directory.Exists(path))
            {
                viewmodel.AddSequence(Sequence.Open(path));
            }
            else if (File.Exists(path))
            {
                list.Add(path);
            }
        }
        viewmodel.AddSequence(Sequence.Open(list));
    }
}