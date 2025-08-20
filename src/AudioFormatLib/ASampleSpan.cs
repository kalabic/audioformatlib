namespace AudioFormatLib;


/// <summary>
/// 
/// Represents a contiguous view into audio SAMPLE memory, without explicit knowledge
/// of a channel layout.
/// 
/// </summary>
public readonly unsafe struct ASampleSpan<T> where T : unmanaged
{
    public static implicit operator AudioSpan(ASampleSpan<T> other) => other.ByteSpan;

    public readonly AudioSpan ByteSpan;


    public AudioPtr Ptr { get { return ByteSpan.Ptr; } }

    public long Offset { get { return ByteSpan.Offset; } }

    public long Length { get { return ByteSpan.Length; } }

    public ACountOf CountOf { get { return ByteSpan.CountOf; } }


    public ASampleSpan(T* ptr, long offset, long length)
    {
        var format = ASampleFormat.NONE.DefaultForType<T>();
        unsafe { ByteSpan = new AudioSpan((byte*)ptr, format.ToByteCount(offset), format.ToByteCount(length), format, 1); }
    }

    public ASampleSpan(T* ptr, long offset, long length, ASampleFormat fmt = ASampleFormat.NONE, int numChannels = 1)
    {
        var format = (fmt == ASampleFormat.NONE) ? fmt.DefaultForType<T>() : fmt;
        unsafe { ByteSpan = new AudioSpan((byte*)ptr, format.ToByteCount(offset), format.ToByteCount(length), format, numChannels); }
    }
}
