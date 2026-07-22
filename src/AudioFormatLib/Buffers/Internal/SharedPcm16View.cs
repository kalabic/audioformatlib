using AudioFormatLib.IO;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal abstract class SharedPcm16View : IPcm16View
{
    internal static bool IsValueCompatible(APcmFormat format)
    {
        return format.SampleValueFormat.IsCompatible<short>();
    }

    internal static bool IsFrameCompatible(APcmFormat format)
    {
        return IsValueCompatible(format) && (!format.IsPlanar || format.ChannelCount <= 1);
    }

    public APcmFormat Format { get; }

    public bool IsOpen => Buffer.IsOpen;

    protected int ValueCapacity => Buffer.CapacityOf<short>();

    protected int ValueCount => Buffer.CountOf<short>();

    protected int FreeValueCapacity =>
        (Buffer.Capacity - Buffer.Count) / sizeof(short);

    protected int FrameCapacity => Buffer.Capacity / Format.BytesPerSampleFrame;

    protected int FrameCount => Buffer.Count / Format.BytesPerSampleFrame;

    protected int FreeFrameCapacity =>
        (Buffer.Capacity - Buffer.Count) / Format.BytesPerSampleFrame;


    protected int ChannelCount => Format.ChannelCount;

    protected IIntegralRingBuffer Buffer { get; }


    protected SharedPcm16View(APcmFormat format, IIntegralRingBuffer buffer)
    {
        Format = format;
        Buffer = buffer;
    }

    protected int GetFrameCount(int valueCount, string parameterName)
    {
        ValidateFrameLayout();

        if (valueCount % ChannelCount != 0)
        {
            throw new ArgumentException(
                "The PCM storage does not contain a complete number of frames.",
                parameterName);
        }

        return valueCount / ChannelCount;
    }

    protected (int ValueOffset, int ValueCount) GetValueRange(
        int frameOffset,
        int frameCount)
    {
        ValidateFrameLayout();
        ArgumentOutOfRangeException.ThrowIfNegative(frameOffset);
        ArgumentOutOfRangeException.ThrowIfNegative(frameCount);

        return (
            checked(frameOffset * ChannelCount),
            checked(frameCount * ChannelCount));
    }

    protected void ValidateFrameLayout()
    {
        if (Format.IsPlanar && ChannelCount > 1)
        {
            throw new NotSupportedException(
                "Frame operations do not support multi-channel planar PCM.");
        }
    }

    protected static unsafe Span<short> CreateSpan(
        short* pointer,
        int offset,
        int count,
        string parameterName)
    {
        ValidatePointer(pointer, offset, count, parameterName);
        return new Span<short>(pointer + offset, count);
    }

    protected static unsafe ReadOnlySpan<short> CreateReadOnlySpan(
        short* pointer,
        int offset,
        int count,
        string parameterName)
    {
        ValidatePointer(pointer, offset, count, parameterName);
        return new ReadOnlySpan<short>(pointer + offset, count);
    }

    private static unsafe void ValidatePointer(
        short* pointer,
        int offset,
        int count,
        string parameterName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (pointer is null && (offset != 0 || count != 0))
        {
            throw new ArgumentNullException(parameterName);
        }
    }
}
