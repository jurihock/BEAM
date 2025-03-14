using System.Runtime.CompilerServices;
using Avalonia.Platform.Storage;
using BEAM.Exporter;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using BEAM.Renderer;
using Moq;

namespace BEAM.Tests.Exporter;

public class PngExporterTests
{
    [Fact]
    public void Export_WritesCorrectData()
    {
        Logger.Init();
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "./TestData/Envi/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");
        var transSequence = new TransformedSequence(sequence);
        
        path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var exportPath = Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "./TestPngFile");
        var pathMock = new Mock<IStorageFile>();
        pathMock.SetupGet(p => p.Path).Returns(new Uri(exportPath));
        var renderer = new ChannelMapRenderer(0, 255, 2, 1, 0);
        
        Directory.Delete(exportPath);
        PngExporter.Export(pathMock.Object, transSequence, renderer);
        
        var sequence2 = SkiaSequence.Open(exportPath);
        Assert.True(SequenceCompare(sequence, sequence2));
        Directory.Delete(exportPath);
    }
    
    [Fact]
    public void Export_WritesCorrectPngFile()
    {
        Logger.Init();
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "./TestData/Envi/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");
        var transSequence = new TransformedSequence(sequence);
        
        path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var exportPath = Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "./TestPngFile");
        var pathMock = new Mock<IStorageFile>();
        pathMock.SetupGet(p => p.Path).Returns(new Uri(exportPath));
        var renderer = new ChannelMapRenderer(0, 255, 2, 1, 0);
        
        Directory.Delete(exportPath);
        EnviExporter.Export(pathMock.Object, transSequence, renderer);
        
        path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var originalPng = Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "./TestData/Png/Test.png");
        
        Assert.True(File.Exists($"{exportPath + Path.DirectorySeparatorChar}00000000.png"));
        Assert.True(FileCompare($"{exportPath + Path.DirectorySeparatorChar}00000000.png", originalPng));
        
        Directory.Delete(exportPath);
    }
    
    // This was copied from the C# tutorial on https://learn.microsoft.com/de-de/troubleshoot/developer/visualstudio/csharp/language-compilers/create-file-compare
    // This method accepts two strings the represent two files to
    // compare. A return value of 0 indicates that the contents of the files
    // are the same. A return value of any other value indicates that the
    // files are not the same.
    private static bool FileCompare(string file1, string file2)
    {
        int file1byte;
        int file2byte;
        FileStream fs1;
        FileStream fs2;

        // Determine if the same file was referenced two times.
        if (file1 == file2)
        {
            // Return true to indicate that the files are the same.
            return true;
        }

        // Open the two files.
        fs1 = new FileStream(file1, FileMode.Open);
        fs2 = new FileStream(file2, FileMode.Open);

        // Check the file sizes. If they are not the same, the files
        // are not the same.
        if (fs1.Length != fs2.Length)
        {
            // Close the file
            fs1.Close();
            fs2.Close();

            // Return false to indicate files are different
            return false;
        }

        // Read and compare a byte from each file until either a
        // non-matching set of bytes is found or until the end of
        // file1 is reached.
        do
        {
            // Read one byte from each file.
            file1byte = fs1.ReadByte();
            file2byte = fs2.ReadByte();
        }
        while ((file1byte == file2byte) && (file1byte != -1));

        // Close the files.
        fs1.Close();
        fs2.Close();

        // Return the success of the comparison. "file1byte" is
        // equal to "file2byte" at this point only if the files are
        // the same.
        return ((file1byte - file2byte) == 0);
    }

    private static bool SequenceCompare(ISequence sequence1, ISequence sequence2)
    {
        if (sequence1.Shape != sequence2.Shape)
        {
            return false;
        }
        
        for (var i = 0; i < sequence1.Shape.Height; i++)
        {
            for(var j = 0; j < sequence1.Shape.Width; j++)
            {
                var same = true;
                for (var k = 0; k < sequence1.Shape.Channels; k++)
                {
                    if (sequence1.GetPixel(j, i)[k] != sequence2.GetPixel(j, i)[k])
                    {
                        same = false;
                        break;
                    }
                }
                if (!same)
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}