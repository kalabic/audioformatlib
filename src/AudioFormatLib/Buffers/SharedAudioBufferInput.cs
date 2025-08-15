using AudioFormatLib.IO;

namespace AudioFormatLib.Buffers;


internal class SharedAudioBufferInput : IAudioBufferInput
{
    public AFrameFormat Format { get { return _format; } }



    private readonly AFrameFormat _format;

    /// <summary> Externally managed buffer. </summary>
    private readonly IUnsafeBuffer _buffer;

    public SharedAudioBufferInput(AFrameFormat format, IUnsafeBuffer sharedBuffer)
    {
        _format = format;
        _buffer = sharedBuffer;
    }

    public void ClearBuffer()
    {
        _buffer.ClearBuffer();
    }

    public int Write(byte[] buffer, int offset, int count)
    {
        return _buffer.Write(buffer, offset, count);
    }

    public int Write(short[] buffer, int offset, int count)
    {
        unsafe
        {
            fixed(short* bufferPtr = buffer)
            {
                return _buffer.Write((byte *)bufferPtr, offset * sizeof(short), count * sizeof(short)) / sizeof(short);
            }
        }
    }
}
