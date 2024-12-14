using System.Net;
using BEAM.Models.LoggerModels;
using BEAM.ViewModels.LogViewModel;

namespace BEAM.Tests;

public class LoggerTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var logger = new Logger("../../../testLog.txt", new LogViewModel());
        
        // Act
        logger.Error(LogEvent.FileNotFound, "Test");

        // Assert
        Assert.True(File.Exists("../../../testLog.txt"));
    }
    
}