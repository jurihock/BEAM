using System;
using System.Runtime.CompilerServices;

namespace BEAM.Exceptions;

/// <summary>
/// Critical exception for BEAM -> do not catch, unrecoverable error
/// </summary>
public class CriticalBeamException(
    string message,
    [CallerLineNumber] int lineNum = 0,
    [CallerFilePath] string path = "")
    : BeamException($"{path}:{lineNum} -> {message}");