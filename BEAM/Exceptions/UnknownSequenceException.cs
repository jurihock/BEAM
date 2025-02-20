namespace BEAM.Exceptions;

/// <summary>
/// Exception thrown when the sequence type is not detectable or unsupported.
/// </summary>
public class UnknownSequenceException : BeamException
{
    public UnknownSequenceException()
    {
    }

    public UnknownSequenceException(string message) : base(message)
    {
    }
}