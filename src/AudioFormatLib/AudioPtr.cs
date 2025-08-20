using AudioFormatLib.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AudioFormatLib;


/// <summary>
/// 
/// Just a byte pointer with information about audio in memory it is referencing.
/// 
/// </summary>
public unsafe readonly struct AudioPtr
{
    public readonly byte* BytePtr;

    public readonly AFrameFormat Fmt;


    public bool IsNull { get { return BytePtr == null; } }

    public AudioPtr()
    {
        BytePtr = null;
        Fmt = AFrameFormat.NONE;
    }

    public AudioPtr(byte* ptr, AFrameFormat fmt)
    {
        BytePtr = ptr;
        Fmt = fmt;
    }

    public T* GetAs<T>() where T : unmanaged
    {
        return (T*)BytePtr;
    }

    public static implicit operator byte*(in AudioPtr other) { return (byte*)other.BytePtr; }

    public static implicit operator short*(in AudioPtr other) { return (short*)(byte*)other.BytePtr; }



    /// <summary>
    /// 
    /// Special catch-all basket for wrapping up different input/output types.
    /// 
    /// </summary>
    public readonly unsafe struct Any<T>
        where T : unmanaged
    {
        public static implicit operator Any<T>(T* other) => new(other);

        public static implicit operator Any<T>(T[] other) => new(other);

        /// <summary> The GC owns Arrays. </summary>
        public readonly T[]? Arr;

        /// <summary> The caller owns pointers. </summary>
        public readonly T* Ptr;

        /// <summary> Default value determined by template parameter T. </summary>
        public readonly ASampleFormat Format;


        public bool IsArray { get { return Arr is not null; } }

        public Any(T* other, ASampleFormat format = ASampleFormat.NONE)
        {
            Arr = null;
            Ptr = other;
            Format = (!GenericType<T>.IsByte && format == ASampleFormat.NONE) ? ASampleFormat.NONE.DefaultForType<T>() : format;
            Debug.Assert(GenericType<T>.IsByte || Format.IsCompatible<T>(), $"Type {nameof(T)} cannot hold sample format given as parameter.");
        }

        public Any(T[] other, ASampleFormat format = ASampleFormat.NONE)
        {
            Arr = other;
            Ptr = default;
            Format = (!GenericType<T>.IsByte && format == ASampleFormat.NONE) ? ASampleFormat.NONE.DefaultForType<T>() : format;
            Debug.Assert(GenericType<T>.IsByte || Format.IsCompatible<T>(), $"Type {nameof(T)} cannot hold sample format given as parameter.");
        }

        /// <summary> Returns an object that MUST be disposed. </summary>
        public Fixed<T> MakeFixed()
        {
            return new Fixed<T>(this);
        }
    }


    /// <summary>
    /// 
    /// The caller-owned disposable pointer.
    /// 
    /// </summary>
    public readonly struct Fixed<T> : IDisposable
        where T : unmanaged
    {
        public static implicit operator byte*(Fixed<T> other) => (byte*)other.Ptr;

        private readonly GCHandle Handle;

        public readonly T* Ptr;

        public readonly ASampleFormat Format;

        internal Fixed(Any<T> other)
        {
            if (other.Arr is not null)
            {
                Handle = GCHandle.Alloc(other.Arr, GCHandleType.Pinned); // Pin permanently
                Ptr = (T*)Handle.AddrOfPinnedObject();
                Format = other.Format;
            }
            else
            {
                Handle = default;
                Ptr = other.Ptr;
                Format = other.Format;
            }
        }

        public void Dispose()
        {
            if (Handle.IsAllocated)
            {
                Handle.Free();
            }
        }
    }
}
