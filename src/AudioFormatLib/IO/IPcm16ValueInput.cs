namespace AudioFormatLib.IO;


/// <summary>
/// Writes signed 16-bit PCM values using the byte order declared by
/// <see cref="IPcm16View.Format"/>. A value is one <see cref="short"/> for one channel at one
/// instant. Ordinary methods may transfer fewer values than requested; <c>Try</c> methods are
/// immediate and all-or-nothing. Offsets, counts, capacities, and return values are measured in
/// values across all channels.
/// </summary>
public interface IPcm16ValueInput : IPcm16View
{
    /// <summary>Total capacity measured in values.</summary>
    int Capacity { get; }

    /// <summary>Number of stored values.</summary>
    int Count { get; }

    /// <summary>Number of additional values that fit.</summary>
    int FreeCapacity { get; }

    int Write(short[] source, int valueOffset, int valueCount);

    unsafe int Write(short* source, int valueOffset, int valueCount);

    int Write(ReadOnlySpan<short> source);

    bool TryWrite(short[] source, int valueOffset, int valueCount);

    unsafe bool TryWrite(short* source, int valueOffset, int valueCount);

    bool TryWrite(ReadOnlySpan<short> source);
}
