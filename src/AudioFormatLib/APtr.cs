using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AudioFormatLib;


/// <summary>
/// 
/// Lightweight wrapper for native pointer type with few helpers.
/// 
/// </summary>
public unsafe readonly struct Ptr_t<T> where T : unmanaged
{
    public static readonly bool IsByte = typeof(T) == typeof(byte);

    public static readonly int SizeOf = sizeof(T);

    public readonly T* Ptr;

    /// <summary> Avoid this. </summary>
    public Ptr_t() { Debug.Assert(false); Ptr = null; }

    public Ptr_t(T* p)
    {
        Ptr = p;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T*(Ptr_t<T> other) => other.Ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Ptr_t<T>(T* other) => new Ptr_t<T>(other);
}


public interface IPtr<T>
    where T : unmanaged
{
    unsafe T* Ptr { get; }

    unsafe T* OffsetPtr { get; }

    long Offset { get; }

    long Length { get; }
}


/// <summary>
/// 
/// A span of samples in memory.
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public unsafe readonly struct APtr<T> : IPtr<T>
    where T : unmanaged
{
    public unsafe T* Ptr { get { return _value.Item1.Ptr; } }

    public unsafe T* OffsetPtr { get { return _value.Item1.Ptr + _value.Item2; } }

    public long Offset { get { return _value.Item2; } }

    public long Length { get { return _value.Item3; } }


    private readonly ValueTuple<Ptr_t<T>, long, long> _value;

    public APtr(in ValueTuple<Ptr_t<T>, long, long> other)
    {
        _value = other;
    }

    public static implicit operator APtr<T>(in ValueTuple<Ptr_t<T>, long, long> other)
    {
        return new APtr<T>(other);
    }
}


/// <summary>
/// 
/// A span of samples in an array.
/// 
/// </summary>
public unsafe struct AObjPtr<T> : IPtr<T>
    where T : unmanaged
{
    public unsafe T* Ptr { get { return (T*)_handle.AddrOfPinnedObject(); } }

    public unsafe T* OffsetPtr { get { return (T*)_handle.AddrOfPinnedObject() + _value.Item2; } }

    public long Offset { get { return _value.Item2; } }

    public long Length { get { return _value.Item3; } }


    private readonly ValueTuple<T[], long, long> _value;

    private GCHandle _handle;

    public AObjPtr(in ValueTuple<T[], long, long> other)
    {
        _value = other;
        _handle = GCHandle.Alloc(_value.Item1, GCHandleType.Pinned); // Pin permanently
    }

    public void Unpin()
    {
        if (_handle.IsAllocated)
        {
            _handle.Free(); // Unpin
        }
    }

    public static implicit operator AObjPtr<T>(in ValueTuple<T[], long, long> other)
    {
        return new AObjPtr<T>(other);
    }
}
