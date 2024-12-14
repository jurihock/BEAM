using System;
using System.Threading.Tasks;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;

namespace BEAM.Image.Minimap;

public abstract class MinimapFunction
{
    private Sequence _sequence;
    private bool isGenerated = false;
    private double[] lineValues;
    
    private delegate void MinimapGeneratedEventHandler(object sender, MinimapGeneratedEventArgs e);
    private event MinimapGeneratedEventHandler minimapGenerated;
    
    MinimapFunction(Sequence sequence, MinimapGeneratedEventHandler eventCallbackFunc)
    {
        if (sequence is null || eventCallbackFunc is null)
        {
            throw new ArgumentNullException(nameof(eventCallbackFunc));
        }
        _sequence 
            = sequence;
        minimapGenerated += eventCallbackFunc;
        Task.Run(generateMinimap);
    }

    private void generateMinimap()
    {
        //TODO: do Work with Sequence
        isGenerated = true;
        minimapGenerated.Invoke(this, new MinimapGeneratedEventArgs());
    }

    public double getMinimapValue(long line)
    {
        if (!isGenerated)
        {
            throw new NotImplementedException();
        }

        return 0.0;
    }
    
    
   
    
}