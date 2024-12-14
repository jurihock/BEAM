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
    }

    private void NativeMenuItem_OnClick(object? sender, EventArgs e)
    {
        Console.WriteLine("NativeMenuItem_OnClick");
    }
}