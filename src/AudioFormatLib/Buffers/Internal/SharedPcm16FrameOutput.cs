using AudioFormatLib.IO;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal sealed class SharedPcm16FrameOutput : SharedPcm16View, IPcm16FrameOutput
{
    private readonly bool _waitForCompleteRead;

    internal SharedPcm16FrameOutput(APcmFormat format, IIntegralRingBuffer buffer)
        : base(format, buffer)
    {
        ValidateFrameLayout();
        _waitForCompleteRead = buffer is IWaitableRingBuffer;
    }

    int IPcm16FrameOutput.Capacity => FrameCapacity;

    int IPcm16FrameOutput.Count => FrameCount;

    int IPcm16FrameOutput.FreeCapacity => FreeFrameCapacity;

    public int Read(short[] destination, int frameOffset, int frameCount)
    {
        ArgumentNullException.ThrowIfNull(destination);
        var range = GetValueRange(frameOffset, frameCount);
        return Read(destination.AsSpan(range.ValueOffset, range.ValueCount));
    }

    public unsafe int Read(short* destination, int frameOffset, int frameCount)
    {
        var range = GetValueRange(frameOffset, frameCount);
        return Read(CreateSpan(
            destination,
            range.ValueOffset,
            range.ValueCount,
            nameof(destination)));
    }

    public int Read(Span<short> destination)
    {
        int requestedFrameCount = GetFrameCount(destination.Length, nameof(destination));
        if (_waitForCompleteRead)
        {
            return Buffer.Read(destination) / ChannelCount;
        }

        if (!Buffer.IsOpen)
        {
            return 0;
        }

        int frameCount = Math.Min(requestedFrameCount, FrameCount);
        if (frameCount == 0)
        {
            return 0;
        }

        int valueCount = checked(frameCount * ChannelCount);
        return Buffer.TryRead(destination[..valueCount]) ? frameCount : 0;
    }

    public bool TryRead(short[] destination, int frameOffset, int frameCount)
    {
        ArgumentNullException.ThrowIfNull(destination);
        var range = GetValueRange(frameOffset, frameCount);
        return TryRead(destination.AsSpan(range.ValueOffset, range.ValueCount));
    }

    public unsafe bool TryRead(short* destination, int frameOffset, int frameCount)
    {
        var range = GetValueRange(frameOffset, frameCount);
        return TryRead(CreateSpan(
            destination,
            range.ValueOffset,
            range.ValueCount,
            nameof(destination)));
    }

    public bool TryRead(Span<short> destination)
    {
        GetFrameCount(destination.Length, nameof(destination));
        return Buffer.TryRead(destination);
    }

    public void Advance(int frameCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(frameCount);
        Buffer.AdvanceBy<short>(checked(frameCount * ChannelCount));
    }
}
