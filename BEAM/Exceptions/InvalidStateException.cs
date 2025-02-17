using BEAM.Log;

namespace BEAM.Exceptions;

public class InvalidStateException : BeamException
{
    public InvalidStateException()
    {
    }

    public InvalidStateException(string message) : base(message)
    {
    }

    public InvalidStateException(LogEvent evt, string message) : base(evt, message)
    {
    }
}