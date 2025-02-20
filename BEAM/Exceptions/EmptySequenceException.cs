namespace BEAM.Exceptions;

public class EmptySequenceException : BeamException
{
    public EmptySequenceException()
    {
    }

    public EmptySequenceException(string message) : base(message)
    {
    }
}