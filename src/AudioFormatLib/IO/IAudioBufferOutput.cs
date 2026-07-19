namespace AudioFormatLib.IO;

public interface IAudioBufferOutput
{
    APcmFormat Format { get; }

    int StoredByteCount { get; }

    public int Read(byte[] buffer, int offset, int count);

    public int Read(short[] buffer, int offset, int count);
}
