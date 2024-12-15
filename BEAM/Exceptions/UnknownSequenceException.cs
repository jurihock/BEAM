using System;

namespace BEAM.Exceptions;

public class UnknownSequenceException : BeamException
{
    public UnknownSequenceException()
    {
    }

    public UnknownSequenceException(string message) : base(message)
    {
    }

    public UnknownSequenceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}