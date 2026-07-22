using AudioFormatLib.IO;
using DotBase.Buffers;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal class SharedAudioBufferOutput : IAudioStreamOutput, IAudioBufferOutput
{
    public override APcmFormat Format { get { return _format; } }

    public int StoredByteCount { get { return _buffer.Count; } }



    private readonly APcmFormat _format;

    /// <summary> Externally managed buffer. </summary>
    private readonly IByteRingBuffer _buffer;

    private readonly IPcm16ValueOutput? _pcm16ValueOutput;

    public SharedAudioBufferOutput(
        APcmFormat format,
        IIntegralRingBuffer sharedBuffer,
        IPcm16ValueOutput? pcm16ValueOutput)
    {
        _format = format;
        _buffer = sharedBuffer;
        _pcm16ValueOutput = pcm16ValueOutput;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _buffer.Read(buffer, offset, count);
    }

    public int Read(short[] buffer, int offset, int count)
    {
        return (_pcm16ValueOutput ?? throw new InvalidOperationException(
            "Short-array operations require signed 16-bit PCM."))
            .Read(buffer, offset, count);
    }
}
