using BEAM.Log;

namespace BEAM.Exceptions;

public class EmptySequenceException : BeamException
{
    public EmptySequenceException() : base()
    {
    }

    public EmptySequenceException(string message) : base(message)
    {
    }

    public EmptySequenceException(LogEvent evt, string message) : base(evt, message)
    {
    }
}