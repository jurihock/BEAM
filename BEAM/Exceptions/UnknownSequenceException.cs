using BEAM.Log;

namespace BEAM.Exceptions;

public class UnknownSequenceException : BeamException
{
    public UnknownSequenceException()
    {
    }

    public UnknownSequenceException(string message) : base(message)
    {
    }

    public UnknownSequenceException(LogEvent evt, string message) : base(evt, message)
    {
    }
}