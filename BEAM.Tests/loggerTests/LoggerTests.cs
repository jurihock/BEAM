using System.Net;
using BEAM.Models.LoggerModels;
using BEAM.ViewModels.LogViewModel;

namespace BEAM.Tests;

public class LoggerTests
{
    [Fact]
    public void CreateNewLogger()
    {
        // Arrange
        var logger = new Logger("../../../loggerTests/testLogs/testLog.txt");
        
        // Act
        logger.Error(LogEvent.FileNotFound, "Test");
        
        logger.Warning(LogEvent.UnknownFileFormat);
        
        logger.Info(LogEvent.ClosedFile, "A File was closed");
        
        logger.LogMessage("This is a test log message");

        // Assert
        Assert.True(File.Exists("../../../loggerTests/testLogs/testLog.txt"));
    }
    

}