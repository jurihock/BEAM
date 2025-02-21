using System;
using System.IO;
using Avalonia.Platform.Storage;
using BEAM.Image.Envi;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using BEAM.Renderer;

namespace BEAM.Exporter;

/// <summary>
/// Provides functionality to export image sequences in the ENVI format.
/// </summary>
public static class EnviExporter
{
    /// <summary>
    /// Exports the given sequence to the specified path in the ENVI format.
    /// </summary>
    /// <param name="path">The path where the files will be saved.</param>
    /// <param name="sequence">The sequence to be exported.</param>
    /// <param name="renderer">The renderer used for the sequence.</param>
    public static void Export(IStorageFile path, TransformedSequence sequence, SequenceRenderer renderer)
    {

        CreateHeaderFile(path, sequence);
        using var stream = File.OpenWrite($"{path.Path.LocalPath}.raw");
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
        Logger.GetInstance().LogMessage($"Finished exporting sequence {sequence.GetName()} as ENVI to {path.Path.LocalPath}");
    }

    /// <summary>
    /// Creates the header file for the ENVI export.
    /// </summary>
    /// <param name="path">The folder where the header file will be saved.</param>
    /// <param name="sequence">The sequence for which the header file is created.</param>
    private static void CreateHeaderFile(IStorageFile path, TransformedSequence sequence)
    {
        var samples = sequence.Shape.Width;
        var lines = sequence.Shape.Height;
        var bands = sequence.Shape.Channels;
        const int headerOffset = 0;
        const string fileType = "ENVI Standard";
        const int dataType = (int)EnviDataType.Double;
        const EnviInterleave interleave = EnviInterleave.BIP;
        const int byteOrder = 0;
        //var header = $"ENVI\nsamples = {samples}\nlines = {lines}\nbands = {bands}\nheader offset = {headerOffset}\nfile type = {fileType}\ndata type = {dataType}\ninterleave = {interleave.ToString().ToLower()}\nbyte order = {byteOrder}";
        var header = $"ENVI{Environment.NewLine}samples = {samples}{Environment.NewLine}lines = {lines}{Environment.NewLine}bands = {bands}{Environment.NewLine}header offset = {headerOffset}{Environment.NewLine}file type = {fileType}{Environment.NewLine}data type = {dataType}{Environment.NewLine}interleave = {interleave.ToString().ToLower()}{Environment.NewLine}byte order = {byteOrder}";
        using var stream = File.OpenWrite($"{path.Path.LocalPath}.hdr");
        using var writer = new StreamWriter(stream);
        writer.Write(header);
    }
}