using AudioFormatLib.Utils;

namespace AudioFormatLib.Tests;

public sealed class Pcm16LittleEndianTests
{
    [Fact]
    public void EncodeDecode_RoundTripsSignedBoundaryValues()
    {
        short[] values = [short.MinValue, -1, 0, 1, short.MaxValue];

        byte[] encoded = Pcm16LittleEndian.Encode(values);
        short[] decoded = Pcm16LittleEndian.Decode(encoded);

        Assert.Equal(values, decoded);
        Assert.Equal(0x00, encoded[0]);
        Assert.Equal(0x80, encoded[1]);
        Assert.Equal(0xFF, encoded[2]);
        Assert.Equal(0xFF, encoded[3]);
    }

    [Fact]
    public void Decode_RejectsOddByteCount()
    {
        Assert.Throws<ArgumentException>(() => Pcm16LittleEndian.Decode([0x01]));
    }
}
