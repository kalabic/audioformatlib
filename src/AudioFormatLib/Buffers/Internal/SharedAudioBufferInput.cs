using AudioFormatLib.IO;
using DotBase.Buffers;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal class SharedAudioBufferInput : IAudioBufferInput
{
    public APcmFormat Format { get { return _format; } }



    private readonly APcmFormat _format;

    /// <summary> Externally managed buffer. </summary>
    private readonly IByteRingBuffer _buffer;

    private readonly IPcm16ValueInput? _pcm16ValueInput;

    public SharedAudioBufferInput(
        APcmFormat format,
        IIntegralRingBuffer sharedBuffer,
        IPcm16ValueInput? pcm16ValueInput)
    {
        _format = format;
        _buffer = sharedBuffer;
        _pcm16ValueInput = pcm16ValueInput;
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
        return (_pcm16ValueInput ?? throw new InvalidOperationException(
            "Short-array operations require signed 16-bit PCM."))
            .Write(buffer, offset, count);
    }
}
