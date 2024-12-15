using System;

namespace BEAM.Exceptions;

/// <summary>
/// This error is thrown when a channel is referenced that does not exist or when the intensity of
/// a channel value is unexpected / illegal.
/// </summary>
public class ChannelException : Exception
{
    public ChannelException() : base()
    {
    }

    public ChannelException(string message) : base(message)
    {
    }

    public ChannelException(string message, Exception inner) : base(message, inner)
    {
    }
}