using AudioFormatLib.IO;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal sealed class SharedPcm16ValueOutput : SharedPcm16View, IPcm16ValueOutput
{
    internal SharedPcm16ValueOutput(APcmFormat format, IIntegralRingBuffer buffer)
        : base(format, buffer)
    {
    }

    int IPcm16ValueOutput.Capacity => ValueCapacity;

    int IPcm16ValueOutput.Count => ValueCount;

    int IPcm16ValueOutput.FreeCapacity => FreeValueCapacity;

    public int Read(short[] destination, int valueOffset, int valueCount)
    {
        ArgumentNullException.ThrowIfNull(destination);
        return Buffer.Read<short>(destination, valueOffset, valueCount);
    }

    public unsafe int Read(short* destination, int valueOffset, int valueCount)
    {
        return Buffer.Read(destination, valueOffset, valueCount);
    }

    public int Read(Span<short> destination)
    {
        return Buffer.Read(destination);
    }

    public bool TryRead(short[] destination, int valueOffset, int valueCount)
    {
        ArgumentNullException.ThrowIfNull(destination);
        return Buffer.TryRead<short>(destination, valueOffset, valueCount);
    }

    public unsafe bool TryRead(short* destination, int valueOffset, int valueCount)
    {
        return Buffer.TryRead(destination, valueOffset, valueCount);
    }

    public bool TryRead(Span<short> destination)
    {
        return Buffer.TryRead(destination);
    }

    public void Advance(int valueCount)
    {
        Buffer.AdvanceBy<short>(valueCount);
    }
}
