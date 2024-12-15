using System.Collections.Generic;
using System.IO;
using BEAM.Image;
using BEAM.Image.Skia;
using BEAM.Log;

namespace BEAM.ImageSequence;

/// <summary>
/// Implementation details for skia images.
/// </summary>
/// <param name="imagePaths">The skia images to use inside the sequence</param>
public class SkiaSequence(List<string> imagePaths) : Sequence(imagePaths)
{
    protected override IContiguousImage LoadImage(int index) => new RgbSkiaImage(imagePaths[index]);

    protected override void InitializeSequence()
    {
        int removed = imagePaths.RemoveAll(path => !Path.GetExtension(path).Equals(".png"));
        if (removed > 0) Logger.GetInstance().Info(LogEvent.OpenedFile, $"{removed} file(s) were not loaded into the sequence.");
    }
}