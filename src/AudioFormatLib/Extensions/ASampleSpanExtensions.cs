namespace AudioFormatLib.Extensions;

public static class ASampleSpanExtensions
{
    public static unsafe ASampleSpan<T> AsSampleSpan<T>(
        this Ptr_t<T> ptr_t,
        int length,
        int numChannels = 1) where T : unmanaged
    {
        return new(ptr_t.Ptr, 0, length, ASampleFormat.NONE, numChannels);
    }

    public static unsafe AudioSpan AsAudioSpan(
        this Ptr_t<byte> ptr_t,
        int length,
        ASampleFormat format,
        int channels = 1)
    {
        return new(ptr_t, 0, length, format, channels);
    }
}
