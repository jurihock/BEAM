using System;

namespace BEAM.Exceptions;

public class EmptySequenceException : BeamException
{
    public EmptySequenceException() {}
    public EmptySequenceException(string message) : base(message) {}
    public EmptySequenceException(string message, Exception inner): base(message, inner) {}
}