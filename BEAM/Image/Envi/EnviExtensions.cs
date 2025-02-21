using System;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace BEAM.Image.Envi;

/// <summary>
/// This class provides utility Methods for other Envi-associated classes like <see cref="EnviByteOrder"/> and
/// <see cref="EnviDataType"/>.
/// </summary>
public static class EnviExtensions
{
    /// <summary>
    /// Returns the host systems native byte order (Little/big-Endian).
    /// </summary>
    /// <returns>The host system's native byte order as a <see cref="EnviByteOrder"/>.</returns>
    public static EnviByteOrder GetNativeByteOrder()
    {
        return BitConverter.IsLittleEndian
            ? EnviByteOrder.Host
            : EnviByteOrder.Network;
    }

    /// <summary>
    /// Checks whether this byte order equals the host's byte order.
    /// </summary>
    /// <param name="order">The byte order to check.</param>
    /// <returns>A Boolean representing whether the byte order is the same as the host's.</returns>
    public static bool IsNative(this EnviByteOrder order)
    {
        return order == GetNativeByteOrder();
    }

    /// <summary>
    /// Checks whether this <see cref="EnviDataType"/> represents complex numbers.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>A Boolean representing whether the data type represents complex numbers.</returns>
    public static bool IsComplex(this EnviDataType type)
    {
        return type switch
        {
            EnviDataType.ComplexSingle => true,
            EnviDataType.ComplexDouble => true,
            _ => false
        };
    }

    /// <summary>
    /// Returns the primitive c# type which corresponds to this data type.
    /// </summary>
    /// <param name="type">The type whose corresponding primitive c# type is meant to be returned.</param>
    /// <returns>The corresponding primitive c# type.</returns>
    /// <exception cref="NotSupportedException">If the type can not be matched to an existing primitive c# type.</exception>
    public static Type TypeOf(this EnviDataType type)
    {
        return type switch
        {
            EnviDataType.Byte => typeof(byte),
            EnviDataType.UInt16 => typeof(ushort),
            EnviDataType.Int16 => typeof(short),
            EnviDataType.UInt32 => typeof(uint),
            EnviDataType.Int32 => typeof(int),
            EnviDataType.UInt64 => typeof(ulong),
            EnviDataType.Int64 => typeof(long),
            EnviDataType.Single => typeof(float),
            EnviDataType.Double => typeof(double),
            _ => throw new NotSupportedException(
                $"Unsupported ENVI data type \"{type}\"!")
        };
    }

    /// <summary>
    /// Returns this data type's size per instance in bytes.
    /// </summary>
    /// <param name="type">The type whose size is meant to be returned. </param>
    /// <returns>This <see cref="EnviDataType"/>'s size in bytes.</returns>
    public static int SizeOf(this EnviDataType type)
    {
        return Marshal.SizeOf(type.TypeOf());
    }



    /// <summary>
    /// Creates a function which takes an offset to a file as an argument and returns an object of the EnviDataType's corresponding primitive c# type.
    /// The offset determines how many readable object are skipped. This means that an offset of 2 means the third readable object is being returned.
    /// E.g. type is int32 and offset is 3. The bytes 13-16 (4*3 + 1 - 4*3+4 | 4 = sizeof(int32)) are being read, interpreted as an integer and returned.
    /// </summary>
    /// <param name="accessor">An accessor to a memory mapped File.</param>
    /// <param name="type">An EnviDataType whose value should be read from the file represented by the accessor.</param>
    /// <typeparam name="T">A specific c# type. The data in the accessor's file will be read as if it were representing this type.</typeparam>
    /// <returns>The offset'th readable element from the file represented by the accessor of the type which is represented by <see cref="EnviDataType"/>.</returns>
    public static Func<long, T> CreateValueGetter<T>(this MemoryMappedViewAccessor accessor, EnviDataType type)
    {
        var instance = Expression.Constant(accessor);
        var method = instance.Type.GetMethod($"Read{type.TypeOf().Name}")!;

        var index = Expression.Parameter(typeof(long));
        var bytes = Expression.Constant((long)type.SizeOf());
        var offset = Expression.Multiply(index, bytes);

        var input = Expression.Call(instance, method, offset);
        var output = Expression.Convert(input, typeof(T));

        return Expression.Lambda<Func<long, T>>(output, index).Compile();
    }
}