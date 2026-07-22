namespace AudioFormatLib.IO;


/// <summary>
/// Writes complete signed 16-bit PCM frames using the byte order declared by
/// <see cref="IPcm16View.Format"/>. A frame contains one interleaved value per channel at one
/// instant. Ordinary methods may transfer fewer frames than requested; <c>Try</c> methods are
/// immediate and all-or-nothing. Array and pointer offsets, counts, capacities, and return values
/// are measured in frames. Span lengths are measured in values, must be divisible by
/// <see cref="APcmFormat.ChannelCount"/>, and must therefore contain a complete number of frames.
/// <para>
/// The buffer does not retain frame metadata. Callers that mix frame operations with raw-byte or
/// value operations must preserve the interleaved frame alignment of the shared cursor.
/// </para>
/// </summary>
public interface IPcm16FrameInput : IPcm16View
{
    /// <summary>Total capacity measured in complete frames.</summary>
    int Capacity { get; }

    /// <summary>Number of stored complete frames.</summary>
    int Count { get; }

    /// <summary>Number of additional complete frames that fit.</summary>
    int FreeCapacity { get; }

    int Write(short[] source, int frameOffset, int frameCount);

    unsafe int Write(short* source, int frameOffset, int frameCount);

    int Write(ReadOnlySpan<short> source);

    bool TryWrite(short[] source, int frameOffset, int frameCount);

    unsafe bool TryWrite(short* source, int frameOffset, int frameCount);

    bool TryWrite(ReadOnlySpan<short> source);
}
