namespace BEAM.Models.Log;

public enum LogEvent
{
    FileNotFound,
    UnknownFileFormat,
    OpenedFile,
    ClosedFile,
    ExportedFile,
    BasicMessage,
    ThrownException,
    Critical
}