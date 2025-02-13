using System;
using BEAM.Models.Log;

namespace BEAM.Exceptions;

public class BeamException : Exception
{
    public BeamException()
    {
        var instance = Logger.GetInstance();
        instance.Error(LogEvent.ThrownException,
            $"A {GetType()} occured without message!");
    }

    public BeamException(string message) : base(message)
    {
        var instance = Logger.GetInstance();
        instance.Error(LogEvent.ThrownException, $"{message}");
    }

    public BeamException(LogEvent evt, string message) : base(message)
    {
        var instance = Logger.GetInstance();
        instance.Error(evt, $"{message}");
    }
}