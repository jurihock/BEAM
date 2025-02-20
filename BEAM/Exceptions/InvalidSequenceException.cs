using System;
using BEAM.Models.Log;

namespace BEAM.Exceptions;

/// <summary>
/// Exception thrown when a sequence cannot be loaded for whatever reason.
/// </summary>
public class InvalidSequenceException : BeamException
{
    public InvalidSequenceException()
    {
    }

    public InvalidSequenceException(string message) : base(message)
    {
    }
}