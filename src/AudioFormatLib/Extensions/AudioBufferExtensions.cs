using AudioFormatLib.IO;

namespace AudioFormatLib.Extensions;

public static class AudioBufferExtensions
{
    public static void WriteSampleValuesExactly(
        this IAudioBuffer buffer,
        short[] source,
        int offset,
        int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(source);
        ValidateRange(source.Length, offset, count);
        var input = buffer.Input.Pcm16Values ?? throw new InvalidOperationException(
            "Exact short-array operations require signed 16-bit PCM.");

        if (!input.TryWrite(source, offset, count))
        {
            throw new InvalidOperationException(
                "The audio buffer is closed or does not have enough free space.");
        }
    }

    public static void ReadSampleValuesExactly(
        this IAudioBuffer buffer,
        short[] destination,
        int offset,
        int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(destination);
        ValidateRange(destination.Length, offset, count);
        var output = buffer.Output.Pcm16Values ?? throw new InvalidOperationException(
            "Exact short-array operations require signed 16-bit PCM.");

        if (!output.TryRead(destination, offset, count))
        {
            throw new InvalidOperationException(
                "The audio buffer is closed or does not contain enough sample values.");
        }
    }

    private static void ValidateRange(int length, int offset, int count)
    {
        if (offset < 0 || count < 0 || offset > length - count)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
    }
}
