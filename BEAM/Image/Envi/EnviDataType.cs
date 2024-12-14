using System;

namespace BEAM.Image.Envi;

public enum EnviDataType
{
  Byte = 1,

  Int16 = 2,
  UInt16 = 12,

  Int32 = 3,
  UInt32 = 13,

  Int64 = 14,
  UInt64 = 15,

  Single = 4,
  Double = 5,

  ComplexSingle = 6,
  ComplexDouble = 9,
}
