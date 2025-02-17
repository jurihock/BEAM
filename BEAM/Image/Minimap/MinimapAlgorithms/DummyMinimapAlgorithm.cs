using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Controls;
using BEAM.ImageSequence;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

public class DummyMinimapAlgorithm : IMinimapAlgorithm
{
    private readonly Random _random = new Random();
    public bool AnalyzeSequence(Sequence sequence, CancellationToken ctx)
    {
        Console.WriteLine("USed the dummy algorithm");
        for(int i = 0; i < 10000; i++)
        {
            sequence.GetPixelLineData(i, [1]);
            Task.Delay(1000);
        }
            
        return true;
    }

    public float GetLineValue(long line)
    {
        return _random.NextSingle();
    }

    public string GetName()
    {
        return "Dummy Algorithm";
    }

    public ISaveControl? GetSettingsPopupControl()
    {
        return null;
    }

    public IMinimapAlgorithm Clone()
    {
        return new DummyMinimapAlgorithm();
    }

    public DummyMinimapAlgorithm()
    {
    }
}