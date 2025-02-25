using System;

namespace BEAM.Exceptions;

/// Base for custom exceptions.
public class BeamException : Exception
{
    protected BeamException()
    {
    }

    protected BeamException(string message) : base(message)
    {
    }
}