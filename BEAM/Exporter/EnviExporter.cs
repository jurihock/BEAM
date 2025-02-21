using System;
using System.IO;
using Avalonia.Platform.Storage;
using BEAM.Image.Envi;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using BEAM.Renderer;

namespace BEAM.Exporter;

public class EnviExporter
{
    public static void Export(IStorageFolder? path, string name, TransformedSequence sequence, SequenceRenderer renderer)
    {
        if (path == null)
        {
            return;
        }
        
        CreateHeaderFile(path, name, sequence);
        using var stream = File.OpenWrite(Path.Combine(path.Path.AbsolutePath, $"{name}.raw"));
        using var writer = new BinaryWriter(stream);
        for (var y = 0; y < sequence.Shape.Height; y++)
        {
            for (var x = 0; x < sequence.Shape.Width; x++)
            {
                var data = sequence.GetPixel(x, y);
                for (var k = 0; k < sequence.Shape.Channels; k++)
                {
                    writer.Write(data[k]);
                }
            }
        }
        Logger.GetInstance().LogMessage($"Finished exporting sequence {sequence.GetName()} as ENVI to {path.Path.AbsolutePath}");
    }
    
    private static void CreateHeaderFile(IStorageFolder path,string name, TransformedSequence sequence)
    {
        var samples = sequence.Shape.Width;
        var lines = sequence.Shape.Height;
        var bands = sequence.Shape.Channels;
        const int headerOffset = 0;
        const string fileType = "ENVI Standard";
        const int dataType = (int) EnviDataType.Double;
        const EnviInterleave interleave = EnviInterleave.BIP;
        const int byteOrder = 0;
        var header = $"ENVI\nsamples = {samples}\nlines = {lines}\nbands = {bands}\nheader offset = {headerOffset}\nfile type = {fileType}\ndata type = {dataType}\ninterleave = {interleave.ToString().ToLower()}\nbyte order = {byteOrder}";
        using var stream = File.OpenWrite(Path.Combine(path.Path.AbsolutePath, $"{name}.hdr"));
        using var writer = new StreamWriter(stream);
        writer.Write(header);
    }
}