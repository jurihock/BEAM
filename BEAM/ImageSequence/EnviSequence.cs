using System;
using System.Collections.Generic;
using System.IO;
using BEAM.Image;
using BEAM.Image.Envi;
using BEAM.Log;

namespace BEAM.ImageSequence;

/// <summary>
/// Implementation details for envi images.
/// </summary>
/// <param name="imagePaths">The envi images to use inside the sequence.</param>
public class EnviSequence(List<string> imagePaths) : Sequence(imagePaths)
{
    protected override IContiguousImage LoadImage(int index) => EnviImage.OpenImage(imagePaths[index]);

    protected override void InitializeSequence()
    {
        imagePaths.RemoveAll(path => !Path.GetExtension(path).Equals(".hdr"));

        int removed = imagePaths.RemoveAll(path => !Path.GetExtension(path).Equals(".raw"));
        if (removed > 0) Logger.GetInstance().Info(LogEvent.OpenedFile, $"{removed} file(s) were not loaded into the sequence.");
    }
}