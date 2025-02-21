using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BEAM.ViewModels;

namespace BEAM.Views;

/// <summary>
/// Class representing the code behind of the about window.
/// </summary>
public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        AddHandler(KeyDownEvent, (_, e) =>
        {
            if (e.Key == Key.Escape) Close();
        });
        DataContext = new AboutWindowViewModel();
    }

    private void OpenRepoWebpage(object? sender, RoutedEventArgs e)
    {
        GetTopLevel(this)!.Launcher.LaunchUriAsync(new Uri("https://github.com/beamsoftware/BEAM"));
    }
}