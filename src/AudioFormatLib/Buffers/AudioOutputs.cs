using AudioFormatLib.IO;
using DotBase.Buffers;

namespace AudioFormatLib.Buffers;


internal class AudioOutputs : IAudioOutputs
{
    public APcmFormat Format { get { return _format; } }

    public IAudioBufferOutput Buffer { get { return _bufferOutput; } }

    public IAudioStreamOutput Stream { get { return _streamOutput; } }



    private readonly APcmFormat _format;

    private readonly IAudioBufferOutput _bufferOutput;

    private readonly IAudioStreamOutput _streamOutput;

    internal AudioOutputs(ABufferParams bparams, IByteRingBuffer buffer)
    {
        _format = bparams.Format;
        _bufferOutput = new SharedAudioBufferOutput(bparams.Format, buffer);
        _streamOutput = (IAudioStreamOutput)_bufferOutput;
    }
}
