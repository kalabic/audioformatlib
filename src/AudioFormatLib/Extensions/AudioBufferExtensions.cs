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
        ValidatePcm16(buffer.Format);

        int byteCount = checked(count * sizeof(short));
        if (buffer.AvailableSpace < byteCount)
        {
            throw new InvalidOperationException("The audio buffer does not have enough free space.");
        }

        int written = buffer.Input.Buffer.Write(source, offset, count);
        if (written != count)
        {
            throw new InvalidOperationException(
                $"The audio buffer accepted {written} of {count} sample values after capacity was reserved.");
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
        ValidatePcm16(buffer.Format);

        if (buffer.StoredSampleValueCount < count)
        {
            throw new InvalidOperationException("The audio buffer does not contain enough sample values.");
        }

        int read = buffer.Output.Buffer.Read(destination, offset, count);
        if (read != count)
        {
            throw new InvalidOperationException(
                $"The audio buffer returned {read} of {count} sample values after data was reserved.");
        }
    }

    private static void ValidatePcm16(APcmFormat format)
    {
        if (format.SampleValueFormat != ASampleValueFormat.S16)
        {
            throw new InvalidOperationException("Exact short-array operations require signed 16-bit PCM.");
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
