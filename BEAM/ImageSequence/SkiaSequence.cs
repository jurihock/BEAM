using System.Collections.Generic;
using System.IO;
using BEAM.Image;
using BEAM.Image.Skia;

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
        imagePaths.RemoveAll(path => !Path.GetExtension(path).Equals(".png"));
    }
}