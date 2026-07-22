using AudioFormatLib.Buffers;
using AudioFormatLib.Extensions;

namespace AudioFormatLib.Tests;

public sealed class AudioStreamBufferTests
{
    [Theory]
    [InlineData(1, 16_000, 8_000, 8_000)]
    [InlineData(2, 32_000, 16_000, 8_000)]
    public void CreateForDuration_UsesPerChannelSampleRate(
        int channelCount,
        int expectedBytes,
        int expectedSampleValues,
        int expectedSamples)
    {
        using AudioStreamBuffer buffer = AudioStreamBuffer.CreateForDuration(
            new APcmFormat(ASampleValueFormat.S16, 8_000, channelCount),
            TimeSpan.FromSeconds(1),
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(expectedBytes, buffer.AllocatedSize);
        Assert.Equal(expectedBytes, buffer.AvailableSpace);
        Assert.Equal(0, buffer.StoredSampleValueCount);
        Assert.Equal(0, buffer.StoredSampleCount);

        short[] values = new short[expectedSampleValues];
        buffer.WriteSampleValuesExactly(values, 0, values.Length);

        Assert.Equal(expectedSampleValues, buffer.StoredSampleValueCount);
        Assert.Equal(expectedSamples, buffer.StoredSampleCount);
    }

    [Fact]
    public void ExactWrite_RejectsInsufficientSpaceWithoutWriting()
    {
        using AudioStreamBuffer buffer = AudioStreamBuffer.CreateForDuration(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 1),
            TimeSpan.FromMilliseconds(20),
            cancellationToken: TestContext.Current.CancellationToken);

        short[] values = new short[161];
        Assert.Throws<InvalidOperationException>(
            () => buffer.WriteSampleValuesExactly(values, 0, values.Length));
        Assert.Equal(0, buffer.StoredByteCount);
    }

    [Fact]
    public void ExactRead_RejectsInsufficientDataWithoutConsuming()
    {
        using AudioStreamBuffer buffer = AudioStreamBuffer.CreateForDuration(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 1),
            TimeSpan.FromMilliseconds(20),
            cancellationToken: TestContext.Current.CancellationToken);
        short[] source = [1, 2, 3];
        buffer.WriteSampleValuesExactly(source, 0, source.Length);

        short[] destination = new short[4];
        Assert.Throws<InvalidOperationException>(
            () => buffer.ReadSampleValuesExactly(destination, 0, destination.Length));
        Assert.Equal(source.Length, buffer.StoredSampleValueCount);

        destination = new short[source.Length];
        buffer.ReadSampleValuesExactly(destination, 0, destination.Length);
        Assert.Equal(source, destination);
    }
}
