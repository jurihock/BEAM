using System;
using System.Threading.Tasks;
using BEAM.Image.Minimap.Utility;
using BEAM.ImageSequence;

namespace BEAM.Image.Minimap;

/// <summary>
/// The minimap for a corresponding sequence. It creates an overview over it by calculating specific values based on an algorithm for each line.
/// Must be supplied with a concrete algorithm which is used for calculations.
/// </summary>
public class MinimapCreator
{
    private Sequence _sequence;
    private bool isGenerated = false;
    private float[] lineValues;
    
    private delegate void MinimapGeneratedEventHandler(object sender, MinimapGeneratedEventArgs e);
    private event MinimapGeneratedEventHandler minimapGenerated;
    
    private readonly MinimapAlgorithm _minimapAlgorithm;
    
    /// <summary>
    /// Initializes the minimap creation process. It creates a separately running Task which generates the values.
    /// Therefore the minimap is not instantly ready after this method call ends hence a method which is used as a callback must be supplied.
    /// </summary>
    /// <param name="sequence">The sequence based on which the minimap is based.</param>
    /// <param name="eventCallbackFunc">A function which is invoked once the minimap has finished generating its values.
    /// This is being done through the <see cref="MinimapGeneratedEventHandler"/> event.</param>
    /// <param name="algorithm">The concrete algorithm used for value calculation.</param>
    /// <exception cref="ArgumentNullException">Is any of the parameters is null.</exception>
    MinimapCreator(Sequence sequence, MinimapGeneratedEventHandler eventCallbackFunc, MinimapAlgorithm algorithm)
    {
        if (sequence is null || eventCallbackFunc is null || algorithm is null)
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
    

    /// <summary>
    /// Returns the algorithm calculation based value for a specific line. Commonly used for plotting.
    /// </summary>
    /// <param name="line">The line whose value shall be returned.</param>
    /// <returns>The specified line's calculated value.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public float getMinimapValue(long line)
    {
        if (!isGenerated)
        {
            throw new NotImplementedException();
        }

        return 0.0f;
    }
    
    
   
    
}