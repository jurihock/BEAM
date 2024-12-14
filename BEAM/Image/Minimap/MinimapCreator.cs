using System;
using System.Threading.Tasks;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;

namespace BEAM.Image.Minimap;

public abstract class MinimapCreator
{
    private Sequence _sequence;
    private bool isGenerated = false;
    private float[] lineValues;
    
    private delegate void MinimapGeneratedEventHandler(object sender, MinimapGeneratedEventArgs e);
    private event MinimapGeneratedEventHandler minimapGenerated;
    
    private readonly MinimapAlgorithm _minimapAlgorithm;
    
    MinimapCreator(Sequence sequence, MinimapGeneratedEventHandler eventCallbackFunc, MinimapAlgorithm algorithm)
    {
        if (sequence is null || eventCallbackFunc is null)
        {
            throw new ArgumentNullException(nameof(eventCallbackFunc));
        }
        _minimapAlgorithm = algorithm;
        _sequence = sequence;
        minimapGenerated += eventCallbackFunc;
        Task.Run(generateMinimap);
    }

    private void generateMinimap()
    {
        
        //TODO: do Work with Sequence
        lineValues = _minimapAlgorithm.analyzeSequence(_sequence);
        isGenerated = true;
        minimapGenerated.Invoke(this, new MinimapGeneratedEventArgs());
    }
    

    public float getMinimapValue(long line)
    {
        if (!isGenerated)
        {
            throw new NotImplementedException();
        }

        return 0.0f;
    }
    
    
   
    
}