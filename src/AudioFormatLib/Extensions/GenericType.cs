namespace AudioFormatLib.Extensions;


public static class GenericType<T>
    where T : unmanaged
{
    /// <summary>
    /// 
    /// Enable the JIT to see GenericType<byte>.IsByte is always true, and it will
    /// (hopefully) remove the else branch entirely at JIT time.
    /// 
    /// </summary>
    public static readonly bool IsByte = typeof(T) == typeof(byte);

    public static readonly bool IsDouble = typeof(T) == typeof(double);

    public static readonly bool IsFloat = typeof(T) == typeof(float);

    public static readonly bool IsInt = typeof(T) == typeof(int);

    public static readonly bool IsUInt8 = typeof(T) == typeof(byte);

    public static readonly bool IsLong = typeof(T) == typeof(long);

    public static readonly bool IsShort = typeof(T) == typeof(short);

    public static unsafe int Size() { return sizeof(T); }
}
