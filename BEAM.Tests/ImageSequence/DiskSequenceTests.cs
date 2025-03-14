using BEAM.Image;
using BEAM.ImageSequence;
using Xunit.Abstractions;

namespace BEAM.Tests.ImageSequence;

/// <summary>
/// Test for the class 
/// </summary>
public class DiskSequenceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DiskSequenceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests opening a PNG sequence with only 4 pixels
    /// </summary>
    /// <exception cref="Exception"></exception>
    [Fact]
    public void TestOpeningRgbw4Sequence_OpenAsFile()
    {
        var testSequenceFolder = "";
        try
        {
            // Create the base directory dynamically
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;

            
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
        
        string testSequenceFilePath = Path.Combine(testSequenceFolder, "RGBW_VierPixelSequenz.png");
        List<string> filepaths = [testSequenceFilePath];
        // open the test Sequence
        var diskSequence = DiskSequence.Open(filepaths, "Test");
        Assert.NotNull(diskSequence);
        // test correct pixel values read (BGR)
        Assert.Equal([0, 0, 255, 255], diskSequence.GetPixel(0,0));
        Assert.Equal([0, 255, 0, 255], diskSequence.GetPixel(1,0));
        Assert.Equal([255, 0, 0, 255], diskSequence.GetPixel(0,1));
        Assert.Equal([255, 255, 255, 255], diskSequence.GetPixel(1,1));
        //Assert.Throws<>()
        
        // test correct shape of the Image
        var shape = new ImageShape(2, 2, 4);
        Assert.Equal(shape, diskSequence.Shape);
        
        diskSequence.Dispose();
    }
    
    /// <summary>
    /// Tests opening a PNG sequence consisting of 3 images (each 10x10x4)
    /// </summary>
    /// <exception cref="Exception">If opening the file fails, an exception is thrown</exception>
    [Fact]
    public void TestOpeningRgbwSequenceRgb3ImagesPNG()
    {
        var testSequenceFolder = "";
        try
        {
            // Create the base directory dynamically
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;

            
            // Use a well-defined path test directory
            testSequenceFolder = Path.Combine(baseDirectory, "TestSequence", "TestSequenceRGBPNG");

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

        Assert.Equal(diskSequence.GetLoadedImageCount(), 3);
        // test correct shape of the Sequence
        var shape = new ImageShape(10, 30, 4);
        Assert.Equal(shape, diskSequence.Shape);
        
        // test correct pixel values read (BGR)
        // test access to first image
        Assert.Equal([0, 0, 255, 255], diskSequence.GetPixel(0,0));
        Assert.Equal([0, 0, 255, 255], diskSequence.GetPixel(9,9));

        // test access to second image
        Assert.Equal([0, 255, 0, 255], diskSequence.GetPixel(0, 10));
        Assert.Equal([0, 255, 0, 255], diskSequence.GetPixel(9,19));
        
        // test access to third image
        Assert.Equal([255, 0, 0, 255], diskSequence.GetPixel(0,20));
        Assert.Equal([255, 0, 0, 255], diskSequence.GetPixel(9,29));
        //Assert.Throws<>()
        
        diskSequence.Dispose();
    }
    
    /// <summary>
    /// Tests opening an ENVI sequence version of the sequence tested above
    /// </summary>
    /// <exception cref="Exception"></exception>
    [Fact]
    public void TestOpeningRgbwSequenceRgb3ImagesEnvi()
    {
        var testSequenceFolder = "";
        try
        {
            // Create the base directory dynamically
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;

            
            // Use a well-defined path test directory
            testSequenceFolder = Path.Combine(baseDirectory, "TestSequence", "TestSequenceRGBENVI");

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

        Assert.Equal(diskSequence.GetLoadedImageCount(), 2);
        // test correct shape of the Sequence
        var shape = new ImageShape(10, 30, 4);
        Assert.Equal(shape, diskSequence.Shape);
        
        // test correct pixel values read (BGR)
        // test access to first image
        Assert.Equal([0, 0, 255, 255], diskSequence.GetPixel(0,0));
        Assert.Equal([0, 0, 255, 255], diskSequence.GetPixel(9,9));

        // test access to second image
        Assert.Equal([0, 255, 0, 255], diskSequence.GetPixel(0, 10));
        Assert.Equal([0, 255, 0, 255], diskSequence.GetPixel(9,19));
        
        // test access to third image
        Assert.Equal([255, 0, 0, 255], diskSequence.GetPixel(0,20));
        Assert.Equal([255, 0, 0, 255], diskSequence.GetPixel(9,29));
        //Assert.Throws<>()
        
        diskSequence.Dispose();
    }
}