using AudioFormatLib.IO;

namespace AudioFormatLib.Buffers;


internal class SharedAudioBufferOutput : IAudioStreamOutput, IAudioBufferOutput
{
    /// <summary> Externally managed buffer. </summary>
    private readonly IUnsafeBuffer _buffer;

    public SharedAudioBufferOutput(IUnsafeBuffer sharedBuffer)
    {
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
