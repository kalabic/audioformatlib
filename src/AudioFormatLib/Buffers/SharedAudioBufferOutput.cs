using AudioFormatLib.IO;
using DotBase.Buffers;

namespace AudioFormatLib.Buffers;


internal class SharedAudioBufferOutput : IAudioStreamOutput, IAudioBufferOutput
{
    public override APcmFormat Format { get { return _format; } }

    public int StoredByteCount { get { return _buffer.Count; } }



    private readonly APcmFormat _format;

    /// <summary> Externally managed buffer. </summary>
    private readonly IByteRingBuffer _buffer;

    public SharedAudioBufferOutput(APcmFormat format, IByteRingBuffer sharedBuffer)
    {
        _format = format;
        _buffer = sharedBuffer;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _buffer.Read(buffer, offset, count);
    }

    public int Read(short[] buffer, int offset, int count)
    {
        unsafe
        {
            fixed(short* bufferPtr = buffer)
            {
                return _buffer.Read((byte *)bufferPtr, offset * sizeof(short), count * sizeof(short)) / sizeof(short);
            }
        }
    }
}
