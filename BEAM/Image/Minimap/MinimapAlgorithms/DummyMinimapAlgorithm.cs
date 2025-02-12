using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using BEAM.ImageSequence;

namespace BEAM.Image.Minimap.MinimapAlgorithms;

public class DummyMinimapAlgorithm : IMinimapAlgorithm
{
    private Random random = new Random();
    public bool AnalyzeSequence(Sequence sequence, CancellationToken ctx)
    {
        for(int i = 0; i < 10000; i++)
        {
            sequence.GetPixelLineData(i, [1]);
            Console.WriteLine(i);
        }
            
        return true;
    }

    public float GetLineValue(long line)
    {
        return random.NextSingle();
    }
}