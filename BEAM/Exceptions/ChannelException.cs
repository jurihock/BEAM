using System;
using BEAM.Log;

namespace BEAM.Exceptions;

/// <summary>
/// This error is thrown when a channel is referenced that does not exist or when the intensity of
/// a channel value is unexpected / illegal.
/// </summary>
public class ChannelException : BeamException
{
    public ChannelException() : base()
    {
    }

    public ChannelException(string message) : base(message)
    {
    }

    public ChannelException(LogEvent evt, string message) : base(evt, message)
    {
    }
}