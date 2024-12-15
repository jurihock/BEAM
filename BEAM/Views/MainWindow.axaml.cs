using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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
    }
}