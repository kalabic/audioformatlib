using AudioFormatLib.Utils;

namespace AudioFormatLib.Tests;

public sealed class AudioResamplerTests
{
    [Fact]
    public void ArrayOverload_ProcessesInputAndSupportsEmptyFinalFlush()
    {
        using AudioResampler resampler = AudioResampler.CreatePcm16(8_000, 24_000);
        short[] input = Enumerable.Range(0, 160).Select(value => (short)value).ToArray();

        short[] initial = resampler.Process(input, endOfInput: false);
        short[] flushed = resampler.Process(Array.Empty<short>(), endOfInput: true);

        Assert.InRange(initial.Length + flushed.Length, 480, 481);
    }
}
