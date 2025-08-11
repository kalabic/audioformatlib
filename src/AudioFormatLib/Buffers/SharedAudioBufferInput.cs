using AudioFormatLib.IO;
using System.Runtime.CompilerServices;

namespace AudioFormatLib.Buffers;


internal class SharedAudioBufferInput : IAudioStreamInput, IAudioBufferInput
{
    /// <summary> Externally managed buffer. </summary>
    private readonly IUnsafeBuffer _buffer;

    public SharedAudioBufferInput(IUnsafeBuffer sharedBuffer)
    {
        _buffer = sharedBuffer;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _buffer.Write(buffer, offset, count);
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
