using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BEAM.Models;

namespace BEAM.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }


    private void NativeMenuItem_OnClick(object? sender, EventArgs e)
    {
        Console.WriteLine("NativeMenuItem_OnClick");
    }
}