// (c) Paul Stier, 2025

using System;
using Avalonia.Platform;
using BEAM.Image.Skia;
using SkiaSharp;

namespace BEAM.Image;

/// <summary>
/// A skia image that is being loaded from compile-time assets, using avalonia's asset loader.
/// </summary>
/// <param name="assetUri">The uri of the asset to load</param>
public class AssetImage(string assetUri) : SkiaImage<byte>(AssetLoader.Open(new Uri(assetUri)))
{
    /// <summary>
    /// Returns the skia bitmap that this class representś.
    /// </summary>
    /// <returns></returns>
    public SKBitmap GetBitmap()
    {
        return Data!;
    }
}