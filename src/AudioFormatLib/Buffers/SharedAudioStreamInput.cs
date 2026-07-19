using AudioFormatLib.IO;

namespace AudioFormatLib.Buffers;


internal class SharedAudioStreamInput : IAudioStreamInput
{
    public override APcmFormat Format => throw new NotImplementedException();


    private readonly APcmFormat _format;

    /// <summary> Externally managed buffer. </summary>
    private readonly IUnsafeBuffer _buffer;

    public SharedAudioStreamInput(APcmFormat format, IUnsafeBuffer sharedBuffer)
    {
        _format = format;
        _buffer = sharedBuffer;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _buffer.Write(buffer, offset, count);
    }
}
