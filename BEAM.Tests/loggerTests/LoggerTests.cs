using BEAM.Models.Log;

namespace BEAM.Tests.loggerTests;

public class LoggerTests
{
    [Fact]
    public void CreateNewLogger()
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
        Assert.True(File.Exists("../../../loggerTests/testLogs/testLog.txt"));
    }
    

}