using DotBase.Buffers;

namespace AudioFormatLib;


/// <summary>Specifies the byte order used to store multi-byte PCM sample values.</summary>
public enum AByteOrder
{
    Native,
    LittleEndian,
    BigEndian,
}


internal static class AByteOrderMethods
{
    internal static AByteOrder Resolve(this AByteOrder byteOrder)
    {
        return byteOrder switch
        {
            AByteOrder.Native => BitConverter.IsLittleEndian
                ? AByteOrder.LittleEndian
                : AByteOrder.BigEndian,
            AByteOrder.LittleEndian => AByteOrder.LittleEndian,
            AByteOrder.BigEndian => AByteOrder.BigEndian,
            _ => throw new ArgumentOutOfRangeException(nameof(byteOrder)),
        };
    }

    internal static ByteOrder ToIntegralByteOrder(this AByteOrder byteOrder)
    {
        return byteOrder switch
        {
            AByteOrder.Native => ByteOrder.Native,
            AByteOrder.LittleEndian => ByteOrder.LittleEndian,
            AByteOrder.BigEndian => ByteOrder.BigEndian,
            _ => throw new ArgumentOutOfRangeException(nameof(byteOrder)),
        };
    }
}
