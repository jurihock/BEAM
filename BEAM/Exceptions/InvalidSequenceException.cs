using System;
using BEAM.Models.Log;

namespace BEAM.Exceptions;

/// <summary>
/// Not a BeamException
/// </summary>
public class InvalidSequenceException : BeamException
{
    public InvalidSequenceException()
    {
    }

    public InvalidSequenceException(string message) : base(message)
    {
    }

    public InvalidSequenceException(LogEvent evt, string message) : base(evt, message)
    {
    }
}