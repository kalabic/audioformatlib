using System.Diagnostics;

namespace AudioFormatLib;


/// <summary>
/// 
/// Represents a contiguous view into PCM memory, with explicit knowledge
/// of channel layout, format, and stride rules.
/// 
/// <para>
/// The <see cref="AudioSpan.Ptr"/> field always points directly to the first sample value
/// of channel 0, sample 0, regardless of planar vs interleaved layout. Any offset
/// provided at construction is applied immediately to <see cref="AudioSpan.Ptr"/>. 
/// </para>
/// 
/// <list type="bullet">
///     <item>Each sample contains <c>ChannelCount</c> sample values (one per channel).</item>
///     <item>If <c>Planar</c> is false, data is interleaved by sample.</item>
///     <item>If <c>Planar</c> is true, sample values are arranged as contiguous blocks per channel.</item>
/// </list>
/// 
/// Use <see cref="AtSample{T}(long, int)"/> for layout-agnostic indexing.
/// 
/// </summary>
public readonly unsafe struct AudioSpan
{
    public static readonly AudioSpan Empty = new();


    /// <summary> Audio pointer holds information about format of audio it is referencing. </summary>
    public readonly AudioPtr Ptr;

    /// <summary> Measured in BYTES. </summary>
    public readonly long Offset;

    /// <summary> Measured in BYTES. </summary>
    public readonly long Length;

    /// <summary> Number of bytes, samples, sample values, and channels in the region. </summary>
    public readonly ACountOf CountOf;


    public byte* BytePtr { get { return Ptr.BytePtr; } }

    public APcmFormat Format { get { return Ptr.Fmt; } }

    public ASampleValueFormat SampleValueFormat { get { return Ptr.Fmt.SampleValueFormat; } }

    public long OffsetSampleValues { get { return Offset / CountOf.BytesPerSampleValue; } }

    public long LengthSampleValues { get { return Length / CountOf.BytesPerSampleValue; } }

    public long OffsetSamples { get { return Offset / CountOf.BytesPerSampleFrame; } }

    public long LengthSamples { get { return Length / CountOf.BytesPerSampleFrame; } }

    /// <summary>
    /// 
    /// If parameter <paramref name="channelCount"/> is larger than 1, this constructor will
    /// always assume channel layout is interleaved. For multi-channel planar version use
    /// other constructor where this can be avoided.
    /// 
    /// </summary>
    public AudioSpan(byte* ptr, long offset, long length, ASampleValueFormat fmt, int channelCount = 1)
        : this(ptr, offset, length, fmt, channelCount, (channelCount == 1))
    { }


    /// <summary>
    /// 
    /// Use when underlying PCM has a multi-channel planar layout, and set <paramref name="planar"/>
    /// to <c>true</c>. For interleaved and mono better simply use the other constructor.
    /// 
    /// </summary>
    public AudioSpan(byte* ptr, long offset, long length, ASampleValueFormat fmt, int channelCount, bool planar = false)
        : this(new AudioPtr(ptr, new APcmFormat(fmt, 0, channelCount, planar)), offset, length)
    { }

    public AudioSpan(byte* ptr, long offset, long length, APcmFormat fmt)
        : this(new AudioPtr(ptr, fmt), offset, length)
    { }

    public AudioSpan(in AudioPtr ptr, long offset, long length)
    {
        Debug.Assert(!ptr.IsNull || length == 0);
        Debug.Assert(offset >= 0 && length >= 0);

        Ptr = ptr;
        Length = length;
        Offset = offset;
        CountOf = new ACountOf(length, ptr.Fmt.SampleValueFormat, ptr.Fmt.ChannelCount);
    }

    /// <summary> Parameter <paramref name="index"/> is the linear index of a scalar sample value. </summary>
    public T AtSampleValueIndex<T>(long index) where T : unmanaged
    {
        return *GetSampleValuePtr<T>(index);
    }

    /// <summary> Parameter <paramref name="index"/> is a temporal sample index. </summary>
    public T AtSample<T>(long index, int channel) where T : unmanaged
    {
        return *GetSamplePtr<T>(index, channel);
    }

    /// <summary> Parameter <paramref name="index"/> is the linear index of a scalar sample value. </summary>
    public T* GetSampleValuePtr<T>(long index) where T : unmanaged
    {
        Debug.Assert(0 <= index);
        Debug.Assert(index < CountOf.SampleValues);
        Debug.Assert(SampleValueFormat.IsCompatible<T>());
        return Ptr.GetAs<T>() + (Offset / sizeof(T)) + index;
    }

    /// <summary> Parameter <paramref name="index"/> is a temporal sample index. </summary>
    public T* GetSamplePtr<T>(long index, int channel) where T : unmanaged
    {
        Debug.Assert(0 <= index && 0 <= channel);
        Debug.Assert(index < CountOf.Samples && channel < CountOf.ChannelCount);
        Debug.Assert(SampleValueFormat.IsCompatible<T>());

        if (!Format.IsPlanar)
        {
            return Ptr.GetAs<T>() + (Offset / sizeof(T)) + index * CountOf.ChannelCount + channel;
        }
        else
        {
            return Ptr.GetAs<T>() + (Offset / sizeof(T)) + CountOf.Samples * channel + index;
        }
    }

    public AudioSpan GetSampleSpan(long sampleOffset, long sampleCount)
    {
        return GetSubSpan(sampleOffset * CountOf.BytesPerSampleFrame, sampleCount * CountOf.BytesPerSampleFrame);
    }

    public AudioSpan GetSampleValueSpan(long sampleValueOffset, long sampleValueCount)
    {
        return GetSubSpan(
            sampleValueOffset * CountOf.BytesPerSampleValue,
            sampleValueCount * CountOf.BytesPerSampleValue);
    }

    public AudioSpan GetSubSpan(long byteOffset, long byteLength)
    {
        if (Format.ChannelCount > 1 && Format.IsPlanar)
        {
            throw new NotSupportedException("Manipulation of multi-channel planar PCM samples is not supported.");
        }

        Debug.Assert((Offset + byteOffset + byteLength) <= Length);
        return new AudioSpan(Ptr, Offset + byteOffset, byteLength);
    }
}
