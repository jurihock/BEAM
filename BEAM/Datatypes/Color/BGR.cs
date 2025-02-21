using System;
using System.Runtime.InteropServices;

namespace BEAM.Datatypes.Color;

/// <summary>
/// Class representing an 8-bit BGR color Value.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BGR(byte b, byte g, byte r)
{
  public byte B = b;
  public byte G = g;
  public byte R = r;

  public byte this[int index]
  {
    readonly get
    {
      switch (index)
      {
        case 0: return B;
        case 1: return G;
        case 2: return R;
      }

      throw new ArgumentOutOfRangeException(nameof(index));
    }
    set
    {
      switch (index)
      {
        case 0: B = value; return;
        case 1: G = value; return;
        case 2: R = value; return;
      }

      throw new ArgumentOutOfRangeException(nameof(index));
    }
  }
  public BGR(byte[] input) : this(input[0], input[1], input[2])
  {
    if (input.Length != 3)
    {
      throw new ArgumentException("Input array must have a length of 3.");
    }
  }
}
