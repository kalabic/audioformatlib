namespace AudioFormatLib.Extensions;


public static class GenericType<T>
{
    /// <summary>
    /// 
    /// Enable the JIT to see GenericType<byte>.IsByte is always true, and it will
    /// (hopefully) remove the else branch entirely at JIT time.
    /// 
    /// </summary>
    public static readonly bool IsByte = typeof(T) == typeof(byte);
}
