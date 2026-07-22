using AudioFormatLib.IO;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal sealed class SharedPcm16FrameInput : SharedPcm16View, IPcm16FrameInput
{
    internal SharedPcm16FrameInput(APcmFormat format, IIntegralRingBuffer buffer)
        : base(format, buffer)
    {
        ValidateFrameLayout();
    }

    int IPcm16FrameInput.Capacity => FrameCapacity;

    int IPcm16FrameInput.Count => FrameCount;

    int IPcm16FrameInput.FreeCapacity => FreeFrameCapacity;

    public int Write(short[] source, int frameOffset, int frameCount)
    {
        ArgumentNullException.ThrowIfNull(source);
        var range = GetValueRange(frameOffset, frameCount);
        return Write(source.AsSpan(range.ValueOffset, range.ValueCount));
    }

    public unsafe int Write(short* source, int frameOffset, int frameCount)
    {
        var range = GetValueRange(frameOffset, frameCount);
        return Write(CreateReadOnlySpan(
            source,
            range.ValueOffset,
            range.ValueCount,
            nameof(source)));
    }

    public int Write(ReadOnlySpan<short> source)
    {
        int requestedFrameCount = GetFrameCount(source.Length, nameof(source));
        if (!Buffer.IsOpen)
        {
            return 0;
        }

        int frameCount = Math.Min(requestedFrameCount, FreeFrameCapacity);
        if (frameCount == 0)
        {
            return 0;
        }

        int valueCount = checked(frameCount * ChannelCount);
        return Buffer.TryWrite(source[..valueCount]) ? frameCount : 0;
    }

    public bool TryWrite(short[] source, int frameOffset, int frameCount)
    {
        ArgumentNullException.ThrowIfNull(source);
        var range = GetValueRange(frameOffset, frameCount);
        return TryWrite(source.AsSpan(range.ValueOffset, range.ValueCount));
    }

    public unsafe bool TryWrite(short* source, int frameOffset, int frameCount)
    {
        var range = GetValueRange(frameOffset, frameCount);
        return TryWrite(CreateReadOnlySpan(
            source,
            range.ValueOffset,
            range.ValueCount,
            nameof(source)));
    }

    public bool TryWrite(ReadOnlySpan<short> source)
    {
        GetFrameCount(source.Length, nameof(source));
        return Buffer.TryWrite(source);
    }
}
