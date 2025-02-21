using System;
using BEAM.Exceptions;

namespace BEAM.Datatypes;

public static class ArrayExtensions
{
    /// <summary>
    /// Given an Array of channel intensities, return the first index with the highest intensity.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <exception cref="ChannelException"></exception>
    public static int ArgMax(this double[] values)
    {
        if (values.Length <= 0)
        {
            throw new ArgumentException("Channels must be greater than 0.");
        }
        
        var argindex = -1;
        var maxvalue = double.MinValue;

        for (var i = 0; i < values.Length; i++)
        {
            if (!(values[i] > maxvalue)) continue;
            
            argindex = i;
            maxvalue = values[i];
        }

        return argindex;
    }
}
