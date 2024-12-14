using System;

namespace BEAM.Views.Titlebar;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

public partial class MenuBar : NativeMenu
{
    public MenuBar()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void NativeMenuItem_OnClick(object? sender, EventArgs e)
    {
        Console.WriteLine("Test");
    }
}