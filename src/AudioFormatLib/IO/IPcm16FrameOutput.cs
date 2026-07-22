namespace AudioFormatLib.IO;


/// <summary>
/// Reads complete signed 16-bit PCM frames using the byte order declared by
/// <see cref="IPcm16View.Format"/>. A frame contains one interleaved value per channel at one
/// instant. Ordinary methods may transfer fewer frames than requested unless the owning buffer is
/// waitable; <c>Try</c> methods are immediate and all-or-nothing. Array and pointer offsets,
/// counts, capacities, and return values are measured in frames. Span lengths are measured in
/// values, must be divisible by <see cref="APcmFormat.ChannelCount"/>, and must therefore contain
/// a complete number of frames.
/// <para>
/// The buffer does not retain frame metadata. Callers that mix frame operations with raw-byte or
/// value operations must preserve the interleaved frame alignment of the shared cursor.
/// </para>
/// </summary>
public interface IPcm16FrameOutput : IPcm16View
{
    /// <summary>Total capacity measured in complete frames.</summary>
    int Capacity { get; }

    /// <summary>Number of stored complete frames.</summary>
    int Count { get; }

    /// <summary>Number of additional complete frames that fit.</summary>
    int FreeCapacity { get; }

    int Read(short[] destination, int frameOffset, int frameCount);

    unsafe int Read(short* destination, int frameOffset, int frameCount);

    int Read(Span<short> destination);

    bool TryRead(short[] destination, int frameOffset, int frameCount);

    unsafe bool TryRead(short* destination, int frameOffset, int frameCount);

    bool TryRead(Span<short> destination);

    /// <summary>Discards the specified number of complete frames.</summary>
    void Advance(int frameCount);
}
