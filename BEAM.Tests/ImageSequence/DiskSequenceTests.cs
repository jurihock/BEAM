using BEAM.ImageSequence;
using Xunit.Abstractions;

namespace BEAM.Tests.ImageSequence;

public class DiskSequenceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DiskSequenceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestOpeningRgbwSequence()
    {
        string testSequenceFolder = "";
        try
        {
            // Create the base directory dynamically
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            baseDirectory = Directory.GetParent(baseDirectory).FullName;
            baseDirectory = Directory.GetParent(baseDirectory).FullName;
            baseDirectory = Directory.GetParent(baseDirectory).FullName;
            baseDirectory = Directory.GetParent(baseDirectory).FullName;

            
            // Use a well-defined path test directory
            testSequenceFolder = Path.Combine(baseDirectory, "TestSequence");

            // Check if the directory exists
            if (!Directory.Exists(testSequenceFolder))
            {
                throw new Exception($"Cannot find the folder at: {testSequenceFolder}");
            }

            _testOutputHelper.WriteLine("Folder found successfully!");
        }
        catch (Exception ex)
        {
            _testOutputHelper.WriteLine($"Error: {ex.Message}");
        }
        
        // open the test Sequence
        var diskSequence = DiskSequence.Open(testSequenceFolder);
        Assert.NotNull(diskSequence);
        
        Assert.Equal([255, 0, 0, 255], diskSequence.GetPixel(0,0));
        
        diskSequence.Dispose();
    }
}