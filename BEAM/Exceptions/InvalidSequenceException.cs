using System;
using BEAM.Log;

namespace BEAM.Exceptions;

/// <summary>
/// Not a BeamException
/// </summary>
public class InvalidSequenceException : Exception
{
    public InvalidSequenceException() : base()
    {
    }

    public InvalidSequenceException(string message) : base(message)
    {
    }

    public InvalidSequenceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}