using System;

namespace BEAM.Exceptions;

public class ImageDimensionException : Exception
{
    public ImageDimensionException() { }

    public ImageDimensionException(string message)
        : base(message) { }

    public ImageDimensionException(string message, Exception inner)
        : base(message, inner) { }
}