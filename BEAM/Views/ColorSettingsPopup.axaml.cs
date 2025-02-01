using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BEAM.ViewModels;

namespace BEAM.Views;

public partial class ColorSettingsPopup : Window
{
    public ColorSettingsPopup(SequenceViewModel sequenceViewModel)
    {
        DataContext = new ColorSettingsPopupViewModel(sequenceViewModel);
        InitializeComponent();
    }

    private void NumberTextChanging(object? sender, TextChangedEventArgs e)
    {
        var regex = "[^0-9]";
        if (sender is not TextBox textBox) return;
        textBox.Text = Regex.Replace(textBox.Text, regex, "");
    }

    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}