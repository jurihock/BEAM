using BEAM.Models.Log;

namespace BEAM.Exceptions;

/// Exception thrown when a loaded sequence is actually empty.
public class EmptySequenceException : BeamException
{
    public EmptySequenceException()
    {
    }

    public EmptySequenceException(string message) : base(message)
    {
    }
}