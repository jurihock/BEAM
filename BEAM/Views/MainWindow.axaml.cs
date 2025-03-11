using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using BEAM.ViewModels;

namespace BEAM.Views;

/// <summary>
/// Code behind the main window.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContextChanged += (_, _) =>
        {
            if (!IsInitialized) return;
            var viewmodel = (MainWindowViewModel)DataContext!;
            DockView.DataContext = viewmodel.DockingVm;
        };
        Closed += (_, _) => Environment.Exit(0);

        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        var vm = (MainWindowViewModel)DataContext!;

        var data = e.Data.GetFiles();
        if (data is null) return;

        var list = data.Select(f => f.Path).ToList();

        try
        {
            vm.DockingVm.OpenSequenceView(DiskSequence.Open(list));
        }
        catch (UnknownSequenceException)
        {
            try
            {
                vm.DockingVm.OpenSequenceView(DiskSequence.Open(list[0]));
            }
            catch (UnknownSequenceException)
            {
                Logger.GetInstance().Error(LogEvent.OpenedFile,
                                $"Cannot open dragged-in files since no suitable sequence type found. (Supported sequences: {string.Join(", ", DiskSequence.SupportedSequences)})");
            }

        }
        catch (EmptySequenceException)
        {
            Logger.GetInstance().Info(LogEvent.OpenedFile, "The sequence to be opened is empty");
        }
        catch (InvalidSequenceException)
        {
            // not handled, since the sequence will write a log message
        }
    }
}