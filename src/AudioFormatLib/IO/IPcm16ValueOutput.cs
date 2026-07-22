namespace AudioFormatLib.IO;


/// <summary>
/// Reads signed 16-bit PCM values using the byte order declared by
/// <see cref="IPcm16View.Format"/>. A value is one <see cref="short"/> for one channel at one
/// instant. Ordinary methods may transfer fewer values than requested unless the owning buffer is
/// waitable; <c>Try</c> methods are immediate and all-or-nothing. Offsets, counts, capacities, and
/// return values are measured in values across all channels.
/// </summary>
public interface IPcm16ValueOutput : IPcm16View
{
    /// <summary>Total capacity measured in values.</summary>
    int Capacity { get; }

    /// <summary>Number of stored values.</summary>
    int Count { get; }

    /// <summary>Number of additional values that fit.</summary>
    int FreeCapacity { get; }

    int Read(short[] destination, int valueOffset, int valueCount);

    unsafe int Read(short* destination, int valueOffset, int valueCount);

    int Read(Span<short> destination);

    bool TryRead(short[] destination, int valueOffset, int valueCount);

    unsafe bool TryRead(short* destination, int valueOffset, int valueCount);

    bool TryRead(Span<short> destination);

    /// <summary>Discards the specified number of values.</summary>
    void Advance(int valueCount);
}
