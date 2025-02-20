using System;

namespace BEAM.Exceptions;

public class BeamException : Exception
{
    protected BeamException()
    {
    }

    protected BeamException(string message) : base(message)
    {
    }
}