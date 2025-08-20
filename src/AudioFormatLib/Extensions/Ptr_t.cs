namespace AudioFormatLib.Extensions;

public readonly unsafe struct Ptr_t<T> where T : unmanaged
{
    public static implicit operator Ptr_t<T>(byte* other) => new Ptr_t<T>(other);

    public static implicit operator Ptr_t<T>(T* other) => new Ptr_t<T>(other);

    public static implicit operator byte*(Ptr_t<T> other) => (byte*)other.Ptr;

    public readonly T* Ptr;

    public Ptr_t(byte* ptr) => Ptr = (T*)ptr;

    public Ptr_t(T* ptr) => Ptr = ptr;
}
