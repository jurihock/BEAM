using System;
using System.Collections.Generic;
using System.IO;
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
            list.Add(file.Path.ToString());
        }
        viewmodel.AddSequence(Sequence.Open(list));
    }
}