using System;

namespace BEAM.Image.Envi;

public enum EnviInterleave
{
  /// <summary>
  /// YXZ
  /// </summary>
  BIP = 012,

  /// <summary>
  /// YZX
  /// </summary>
  BIL = 021,

  /// <summary>
  /// ZYX
  /// </summary>
  BSQ = 201,
}
