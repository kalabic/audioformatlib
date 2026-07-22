using AudioFormatLib.IO;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal sealed class SharedPcm16ValueInput : SharedPcm16View, IPcm16ValueInput
{
    internal SharedPcm16ValueInput(APcmFormat format, IIntegralRingBuffer buffer)
        : base(format, buffer)
    {
    }

    int IPcm16ValueInput.Capacity => ValueCapacity;

    int IPcm16ValueInput.Count => ValueCount;

    int IPcm16ValueInput.FreeCapacity => FreeValueCapacity;

    public int Write(short[] source, int valueOffset, int valueCount)
    {
        ArgumentNullException.ThrowIfNull(source);
        return Buffer.Write<short>(source, valueOffset, valueCount);
    }

    public unsafe int Write(short* source, int valueOffset, int valueCount)
    {
        return Buffer.Write(source, valueOffset, valueCount);
    }

    public int Write(ReadOnlySpan<short> source)
    {
        return Buffer.Write(source);
    }

    public bool TryWrite(short[] source, int valueOffset, int valueCount)
    {
        ArgumentNullException.ThrowIfNull(source);
        return Buffer.TryWrite<short>(source, valueOffset, valueCount);
    }

    public unsafe bool TryWrite(short* source, int valueOffset, int valueCount)
    {
        return Buffer.TryWrite(source, valueOffset, valueCount);
    }

    public bool TryWrite(ReadOnlySpan<short> source)
    {
        return Buffer.TryWrite(source);
    }
}
