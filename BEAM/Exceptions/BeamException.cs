using System;

namespace BEAM.Exceptions;

public class BeamException : Exception
{
    public BeamException() : base()
    {
    }

    public BeamException(string message) : base(message)
    {
    }

    public BeamException(string message, Exception inner) : base(message, inner)
    {
    }
}