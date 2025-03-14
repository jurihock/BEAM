using BEAM.Models.Log;

namespace BEAM.Tests.loggerTests;

public class LoggerTests
{
    [Fact]
    public void CreateNewLogger()
    {
        Logger.Init("testlog.txt");
        // Arrange
        var logger = Logger.GetInstance();
        
        // Act
        logger.Error(LogEvent.FileNotFound, "Test");
        
        logger.Warning(LogEvent.UnknownFileFormat, "File format not supported");
        
        logger.Info(LogEvent.ClosedFile, "A File was closed");
        
        logger.LogMessage("This is a test log message");

        // Assert
        Assert.True(File.Exists("testlog.txt"));
    }
    
    [Fact]
    public void CreateNewLoggerWithDefaultPath()
    {
        Logger.Init();
        // Arrange
        var logger = Logger.GetInstance();
        
        // Act
        logger.Error(LogEvent.FileNotFound, "Test");
        
        logger.Warning(LogEvent.UnknownFileFormat, "File format not supported");
        
        logger.Info(LogEvent.ClosedFile, "A File was closed");
        
        logger.LogMessage("This is a test log message");

        // Assert
        Assert.True(File.Exists("log.txt"));
    }

    [Fact]
    public void GetLogEntries_ReturnsCorrectEntries()
    {
        var logger = Logger.Init("testlog.txt");
        
        logger.Error(LogEvent.FileNotFound, "Test");
        logger.Warning(LogEvent.UnknownFileFormat, "File format not supported");
        logger.Info(LogEvent.ClosedFile, "A File was closed");
        logger.LogMessage("This is a test log message");
        
        var entries = logger.GetLogEntries();
        
        Assert.Equal(4, entries.Count);
        
        Assert.Equal(LogLevel.Error, entries[0].Level);
        Assert.Equal(Enum.GetName(LogEvent.FileNotFound), entries[0].Event);
        
        Assert.Equal(LogLevel.Warning, entries[1].Level);
        Assert.Equal(Enum.GetName(LogEvent.UnknownFileFormat), entries[1].Event);
        
        Assert.Equal(LogLevel.Info, entries[2].Level);
        Assert.Equal(Enum.GetName(LogEvent.ClosedFile), entries[2].Event);
        
        Assert.Equal(Enum.GetName(LogEvent.BasicMessage), entries[3].Event);
    }
}