using PureHDF;
using PureHDF.Selections;
using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace BEAM.Image.Envi;

/// <summary>
/// This class provides utility Methods for other Envi-associated classes like <see cref="EnviByteOrder"/> and
/// <see cref="EnviDataType"/>.
/// </summary>
public static class HdfExtensions
{
    /// <summary>
    /// Returns the primitive c# type which corresponds to this data type.
    /// </summary>
    /// <param name="type">The type whose corresponding primitive c# type is meant to be returned.</param>
    /// <returns>The corresponding primitive c# type.</returns>
    /// <exception cref="NotSupportedException">If the type can not be matched to an existing primitive c# type.</exception>
    public static Type TypeOf(this IH5DataType type)
    {
        if (type.Class == H5DataTypeClass.FixedPoint && type.Size == 2)
        {
            return typeof(short);
        }

        throw new NotSupportedException(
            $"Unsupported HDF data type \"{type}\"!");
    }

    /// <summary>
    /// Returns this data type's size per instance in bytes.
    /// </summary>
    /// <param name="type">The type whose size is meant to be returned. </param>
    /// <returns>This <see cref="EnviDataType"/>'s size in bytes.</returns>
    public static int SizeOf(this IH5DataType type)
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
    public static Func<long, long, long, T> CreateValueGetter<T>(this IH5Dataset dataset)
    {
        var type = dataset.Type;

        var instance = Expression.Constant(dataset);
        var method = instance.Type.GetMethod("Read", [typeof(Selection), typeof(Selection), typeof(ulong[])])!.MakeGenericMethod(type.TypeOf());

        var index_ = Expression.Variable(typeof(ulong[,]));
        var index0 = Expression.Parameter(typeof(long));
        var index1 = Expression.Parameter(typeof(long));
        var index2 = Expression.Parameter(typeof(long));

        var assign_ = Expression.Assign(index_, Expression.NewArrayBounds(typeof(ulong), Expression.Constant(1), Expression.Constant(3)));
        var assign0 = Expression.Assign(Expression.ArrayAccess(index_, Expression.Constant(0), Expression.Constant(0)), Expression.Convert(index0, typeof(ulong)));
        var assign1 = Expression.Assign(Expression.ArrayAccess(index_, Expression.Constant(0), Expression.Constant(1)), Expression.Convert(index1, typeof(ulong)));
        var assign2 = Expression.Assign(Expression.ArrayAccess(index_, Expression.Constant(0), Expression.Constant(2)), Expression.Convert(index2, typeof(ulong)));

        var index = Expression.Block([index_], assign_, assign0, assign1, assign2, index_);

        var fileSelection = Expression.New(typeof(PointSelection).GetConstructor([typeof(ulong[,])])!, index);
        var memorySelection = Expression.Constant(null, typeof(Selection));
        var memoryDims = Expression.Constant(null, typeof(ulong[]));

        var input = Expression.Call(instance, method, fileSelection, memorySelection, memoryDims);
        var shift = Expression.RightShift(input, Expression.Constant(8)); // assume 8-bit values left shifted to 16-bit
        var output = Expression.Convert(type.TypeOf() == typeof(short) ? shift : input, typeof(T));

        var indices = new[] { index1 /* X */, index0 /* Y */, index2 /* Z */ }; // assume native YXZ memory layout, thus swap X and Y axes

        return Expression.Lambda<Func<long, long, long, T>>(output, indices).Compile();
    }

    public static Action<long, long, int[], double[]> CreateDoubleValuesGetter(this IH5Dataset dataset)
    {
        var type = dataset.Type;

        if (type.TypeOf() != typeof(short))
        {
            throw new NotSupportedException(
                $"Currently unsupported dataset data type {type.TypeOf()}!");
        }

        return new Action<long, long, int[], double[]>((x, y, zzz, dst) =>
        {
            if (dst.Length != zzz.Length)
            {
                throw new ArgumentException(
                    $"Invalid array shape {dst.Length} != {zzz.Length}!");
            }

            var points = new ulong[zzz.Length, 3];

            for (var i = 0; i < zzz.Length; i++)
            {
                points[i, 0] = (ulong)y;
                points[i, 1] = (ulong)x;
                points[i, 2] = (ulong)zzz[i];
            }

            var src = dataset.Read<short[]>(new PointSelection(points));

            for (var i = 0; i < zzz.Length; i++)
            {
                dst[i] = src[i] >> 8;
            }
        });
    }
}
