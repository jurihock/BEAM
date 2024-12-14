using System;

namespace BEAM.Exceptions;

public class UnsupportedException : BeamException
{
    public UnsupportedException()
    {
    }

    public UnsupportedException(string message) : base(message)
    {
    }

    public UnsupportedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}