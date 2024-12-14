using System;

namespace BEAM.Image.Envi;

/// <summary>
/// This enum is used to represent the hosts byte-order (Endianess).
/// </summary>
public enum EnviByteOrder
{
  /// <summary>
  /// Least significant byte first (Little-Endian).
  /// </summary>
  Host = 0,

  /// <summary>
  /// Most significant byte first (Big-Endian).
  /// </summary>
  Network = 1,
}
