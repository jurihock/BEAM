using System.Runtime.InteropServices;

namespace BEAM.Tests.Image.Envi;

using BEAM.Image.Envi;
using System;
using Xunit;

public class EnviExtensionsTests
{
    [Fact]
    public void GetNativeByteOrder_ReturnsHostOrder_WhenSystemIsLittleEndian()
    {
        if (BitConverter.IsLittleEndian)
        {
            Assert.Equal(EnviByteOrder.Host, EnviExtensions.GetNativeByteOrder());
        }
    }

    [Fact]
    public void GetNativeByteOrder_ReturnsNetworkOrder_WhenSystemIsBigEndian()
    {
        if (!BitConverter.IsLittleEndian)
        {
            Assert.Equal(EnviByteOrder.Network, EnviExtensions.GetNativeByteOrder());
        }
    }

    [Fact]
    public void IsNative_ReturnsTrue_WhenOrderIsHostOrder()
    {
        var order = EnviExtensions.GetNativeByteOrder();
        Assert.True(order.IsNative());
    }

    [Fact]
    public void IsNative_ReturnsFalse_WhenOrderIsNotHostOrder()
    {
        var order = EnviExtensions.GetNativeByteOrder() == EnviByteOrder.Host ? EnviByteOrder.Network : EnviByteOrder.Host;
        Assert.False(order.IsNative());
    }

    [Fact]
    public void IsComplex_ReturnsTrue_ForComplexSingle()
    {
        Assert.True(EnviDataType.ComplexSingle.IsComplex());
    }

    [Fact]
    public void IsComplex_ReturnsTrue_ForComplexDouble()
    {
        Assert.True(EnviDataType.ComplexDouble.IsComplex());
    }

    [Fact]
    public void IsComplex_ReturnsFalse_ForNonComplexTypes()
    {
        Assert.False(EnviDataType.Byte.IsComplex());
    }

    [Fact]
    public void TypeOf_ReturnsCorrectType_ForSupportedDataTypes()
    {
        Assert.Equal(typeof(byte), EnviDataType.Byte.TypeOf());
        Assert.Equal(typeof(ushort), EnviDataType.UInt16.TypeOf());
        Assert.Equal(typeof(short), EnviDataType.Int16.TypeOf());
        Assert.Equal(typeof(uint), EnviDataType.UInt32.TypeOf());
        Assert.Equal(typeof(int), EnviDataType.Int32.TypeOf());
        Assert.Equal(typeof(ulong), EnviDataType.UInt64.TypeOf());
        Assert.Equal(typeof(long), EnviDataType.Int64.TypeOf());
        Assert.Equal(typeof(float), EnviDataType.Single.TypeOf());
        Assert.Equal(typeof(double), EnviDataType.Double.TypeOf());
    }

    [Fact]
    public void TypeOf_ThrowsNotSupportedException_ForUnsupportedDataType()
    {
        var unsupportedType = (EnviDataType)999;
        Assert.Throws<NotSupportedException>(() => unsupportedType.TypeOf());
    }

    [Fact]
    public void SizeOf_ReturnsCorrectSize_ForSupportedDataTypes()
    {
        Assert.Equal(Marshal.SizeOf(typeof(byte)), EnviDataType.Byte.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(ushort)), EnviDataType.UInt16.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(short)), EnviDataType.Int16.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(uint)), EnviDataType.UInt32.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(int)), EnviDataType.Int32.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(ulong)), EnviDataType.UInt64.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(long)), EnviDataType.Int64.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(float)), EnviDataType.Single.SizeOf());
        Assert.Equal(Marshal.SizeOf(typeof(double)), EnviDataType.Double.SizeOf());
    }
}