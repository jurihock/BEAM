using System.Collections.Generic;
using System.IO;
using BEAM.Image;
using BEAM.Image.Skia;
using BEAM.Models.Log;

namespace BEAM.ImageSequence;

/// <summary>
/// Implementation details for skia images.
/// </summary>
/// <param name="imagePaths">The skia images to use inside the sequence</param>
public class SkiaSequence(List<string> imagePaths, string name) : DiskSequence(imagePaths, name)
{
    private readonly List<string> _imagePaths = imagePaths;
    protected internal override IImage LoadImage(int index) => new SkiaImage<byte>(_imagePaths[index]);

    protected internal override bool InitializeSequence()
    {
        var removed = _imagePaths.RemoveAll(path => !Path.GetExtension(path).Equals(".png"));
        if (removed > 0) Logger.GetInstance().Info(LogEvent.OpenedFile, $"{removed} file(s) were not loaded into the sequence.");

        return true;
    }
}