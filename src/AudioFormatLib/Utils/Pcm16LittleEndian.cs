using System.Buffers.Binary;

namespace AudioFormatLib.Utils;

public static class Pcm16LittleEndian
{
    public static short[] Decode(ReadOnlySpan<byte> source)
    {
        if ((source.Length & 1) != 0)
        {
            throw new ArgumentException("PCM16 data must contain complete two-byte sample values.", nameof(source));
        }

        short[] result = new short[source.Length / sizeof(short)];
        for (int index = 0; index < result.Length; index++)
        {
            result[index] = BinaryPrimitives.ReadInt16LittleEndian(
                source.Slice(index * sizeof(short), sizeof(short)));
        }

        return result;
    }

    public static byte[] Encode(ReadOnlySpan<short> source)
    {
        byte[] result = new byte[checked(source.Length * sizeof(short))];
        for (int index = 0; index < source.Length; index++)
        {
            BinaryPrimitives.WriteInt16LittleEndian(
                result.AsSpan(index * sizeof(short), sizeof(short)),
                source[index]);
        }

        return result;
    }
}
