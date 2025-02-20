using System;
using System.Threading;
using System.Threading.Tasks;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.Views.Utility;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

public class DummyMinimapAlgorithm : IMinimapAlgorithm
{
    private readonly Random _random = new Random();
    public bool AnalyzeSequence(ISequence sequence, CancellationToken ctx)
    {
        for(int i = 0; i < 10000; i++)
        {
            sequence.GetPixelLineData(i, [1]);
            Task.Delay(1000, ctx);
        }
            
        return true;
    }

    public double GetLineValue(long line)
    {
        return _random.NextDouble();
    }

    public string GetName()
    {
        return "Dummy Algorithm";
    }

    public SaveUserControl? GetSettingsPopupControl()
    {
        return null;
    }
    

    public IMinimapAlgorithm Clone()
    {
        return new DummyMinimapAlgorithm();
    }

    public void SetRenderer(SequenceRenderer renderer)
    {
    }
    
}