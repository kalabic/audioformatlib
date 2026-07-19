namespace AudioFormatLib.Extensions;

public static class ASampleValueSpanExtensions
{
    public static unsafe ASampleValueSpan<T> AsSampleValueSpan<T>(
        this Ptr_t<T> ptr_t,
        int length,
        int channelCount = 1) where T : unmanaged
    {
        return new(ptr_t.Ptr, 0, length, ASampleValueFormat.NONE, channelCount);
    }

    public static unsafe AudioSpan AsAudioSpan(
        this Ptr_t<byte> ptr_t,
        int length,
        ASampleValueFormat sampleValueFormat,
        int channelCount = 1)
    {
        return new(ptr_t, 0, length, sampleValueFormat, channelCount);
    }
}
