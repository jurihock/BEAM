namespace BEAM.Exceptions;

public class InvalidStateException : BeamException
{
    public InvalidStateException()
    {
    }

    public InvalidStateException(string message) : base(message)
    {
    }

}