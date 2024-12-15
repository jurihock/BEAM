using BEAM.Log;

namespace BEAM.Exceptions;

public class ImageDimensionException : BeamException
{
    public ImageDimensionException()
    {
    }

    public ImageDimensionException(string message) : base(message)
    {
    }

    public ImageDimensionException(LogEvent evt, string message) : base(evt, message)
    {
    }
}