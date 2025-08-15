using AudioFormatLib.IO;

namespace AudioFormatLib.Buffers;


internal class SharedAudioBufferOutput : IAudioStreamOutput, IAudioBufferOutput
{
    public override AFrameFormat Format { get { return _format; } }

    public int StoredByteCount { get { return _buffer.Count; } }



    private readonly AFrameFormat _format;

    /// <summary> Externally managed buffer. </summary>
    private readonly IUnsafeBuffer _buffer;

    public SharedAudioBufferOutput(AFrameFormat format, IUnsafeBuffer sharedBuffer)
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
