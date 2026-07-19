namespace AudioFormatLib;


/// <summary>
/// 
/// Represents a contiguous view into scalar sample-value memory, without explicit knowledge
/// of a channel layout.
/// 
/// </summary>
public readonly unsafe struct ASampleValueSpan<T> where T : unmanaged
{
    public static implicit operator AudioSpan(ASampleValueSpan<T> other) => other.ByteSpan;

    public readonly AudioSpan ByteSpan;


    public AudioPtr Ptr { get { return ByteSpan.Ptr; } }

    public long Offset { get { return ByteSpan.Offset; } }

    public long Length { get { return ByteSpan.Length; } }

    public ACountOf CountOf { get { return ByteSpan.CountOf; } }


    public ASampleValueSpan(T* ptr, long offset, long length)
    {
        var sampleValueFormat = ASampleValueFormat.NONE.DefaultForType<T>();
        unsafe { ByteSpan = new AudioSpan((byte*)ptr, sampleValueFormat.ToByteCount(offset), sampleValueFormat.ToByteCount(length), sampleValueFormat, 1); }
    }

    public ASampleValueSpan(
        T* ptr,
        long offset,
        long length,
        ASampleValueFormat sampleValueFormat = ASampleValueFormat.NONE,
        int channelCount = 1)
    {
        var format = sampleValueFormat == ASampleValueFormat.NONE
            ? sampleValueFormat.DefaultForType<T>()
            : sampleValueFormat;
        unsafe { ByteSpan = new AudioSpan((byte*)ptr, format.ToByteCount(offset), format.ToByteCount(length), format, channelCount); }
    }
}
