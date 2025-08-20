using System.Diagnostics;

namespace AudioFormatLib;


/// <summary>
/// 
/// Represents a contiguous view into audio sample memory, with explicit knowledge
/// of channel layout, format, and stride rules.
/// 
/// <para>
/// The <see cref="AudioSpan.Ptr"/> field always points directly to the first sample
/// of channel 0, frame 0, regardless of planar vs interleaved layout. Any offset
/// provided at construction is applied immediately to <see cref="AudioSpan.Ptr"/>. 
/// </para>
/// 
/// <list type="bullet">
///     <item>Each frame contains <c>NumChannels</c> samples (one per channel).</item>
///     <item>If <c>Planar</c> is false, data is interleaved: frame0 contains channel0..channelN-1, then frame1, etc.</item>
///     <item>If <c>Planar</c> is true, data is arranged as contiguous blocks per channel: channel0 frames, then channel1 frames, ...</item>
/// </list>
/// 
/// Use AtFrame(frameIndex, channel) for layout-agnostic indexing.
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

    /// <summary> Number of bytes, samples, frames, etc. inside the underlying region. </summary>
    public readonly ACountOf CountOf;


    public byte* BytePtr { get { return Ptr.BytePtr; } }

    public AFrameFormat Format { get { return Ptr.Fmt; } }

    public ASampleFormat SampleFormat { get { return Ptr.Fmt.SampleFormat; } }

    public long OffsetSamples { get { return Offset / CountOf.BytesInSample; } }

    public long LengthSamples { get { return Length / CountOf.BytesInSample; } }

    public long OffsetFrames { get { return Offset / CountOf.BytesInFrame; } }

    public long LengthFrames { get { return Length / CountOf.BytesInFrame; } }

    /// <summary>
    /// 
    /// If parameter <paramref name="numChannels"/> is larger than 1, this constructor will
    /// always assume channel layout is interleaved. For multi-channel planar version use
    /// other constructor where this can be avoided.
    /// 
    /// </summary>
    public AudioSpan(byte* ptr, long offset, long length, ASampleFormat fmt, int numChannels = 1)
        : this(ptr, offset, length, fmt, numChannels, (numChannels == 1))
    { }


    /// <summary>
    /// 
    /// Use when underlying audio frame has a multi-channel planar layout, and set <paramref name="planar"/>
    /// to <c>true</c>. For interleaved and mono better simply use the other constructor.
    /// 
    /// </summary>
    public AudioSpan(byte* ptr, long offset, long length, ASampleFormat fmt, int numChannels, bool planar = false)
        : this(new AudioPtr(ptr, new AFrameFormat(fmt, 0, numChannels, planar)), offset, length)
    { }

    public AudioSpan(byte* ptr, long offset, long length, AFrameFormat fmt)
        : this(new AudioPtr(ptr, fmt), offset, length)
    { }

    public AudioSpan(in AudioPtr ptr, long offset, long length)
    {
        Debug.Assert(!ptr.IsNull || length == 0);
        Debug.Assert(offset >= 0 && length >= 0);

        Ptr = ptr;
        Length = length;
        Offset = offset;
        CountOf = new ACountOf(length, ptr.Fmt.SampleFormat, ptr.Fmt.NumChannels);
    }

    /// <summary> Parameter <paramref name="index"/> is channel layout agnostic index of a SAMPLE. </summary>
    public T AtSampleIndex<T>(long index) where T : unmanaged
    {
        return *GetSamplePtr<T>(index);
    }

    /// <summary> Parameter <paramref name="index"/> is index of a FRAME or a SAMPLE INSIDE CHANNEL. </summary>
    public T AtFrame<T>(long index, int channel) where T : unmanaged
    {
        return *GetFramePtr<T>(index, channel);
    }

    /// <summary> Parameter <paramref name="index"/> is channel layout agnostic index of a SAMPLE. </summary>
    public T* GetSamplePtr<T>(long index) where T : unmanaged
    {
        Debug.Assert(0 <= index);
        Debug.Assert(index < CountOf.Samples);
        Debug.Assert(SampleFormat.IsCompatible<T>());
        return Ptr.GetAs<T>() + (Offset / sizeof(T)) + index;
    }

    /// <summary> Parameter <paramref name="index"/> is index of a FRAME or a SAMPLE INSIDE CHANNEL. </summary>
    public T* GetFramePtr<T>(long index, int channel) where T : unmanaged
    {
        Debug.Assert(0 <= index && 0 <= channel);
        Debug.Assert(index < CountOf.Frames && channel < CountOf.Channels);
        Debug.Assert(SampleFormat.IsCompatible<T>());

        if (!Format.IsPlanar)
        {
            return Ptr.GetAs<T>() + (Offset / sizeof(T)) + index * CountOf.Channels + channel;
        }
        else
        {
            return Ptr.GetAs<T>() + (Offset / sizeof(T)) + CountOf.Frames * channel + index;
        }
    }

    public AudioSpan GetFrameSpan(long frameOffset, long frameCount)
    {
        return GetSubSpan(frameOffset * CountOf.BytesInFrame, frameCount * CountOf.BytesInFrame);
    }

    public AudioSpan GetSampleSpan(long sampleOffset, long sampleCount)
    {
        return GetSubSpan(sampleOffset * CountOf.BytesInSample, sampleCount * CountOf.BytesInSample);
    }

    public AudioSpan GetSubSpan(long byteOffset, long byteLength)
    {
        if (Format.NumChannels > 1 && Format.IsPlanar)
        {
            throw new NotSupportedException("Manipulation of multi-channel planar frames is not supported.");
        }

        Debug.Assert((Offset + byteOffset + byteLength) <= Length);
        return new AudioSpan(Ptr, Offset + byteOffset, byteLength);
    }
}
