using System.Runtime.CompilerServices;
using BEAM.Models.Log;

namespace BEAM.Exceptions;

/// <summary>
/// Critical exception for BEAM -> do not catch, unrecoverable error
/// </summary>
public class CriticalBeamException(
    string message,
    [CallerLineNumber] int lineNum = 0,
    [CallerFilePath] string path = "")
    : BeamException(LogEvent.Critical, $"{path}:{lineNum} -> {message}");