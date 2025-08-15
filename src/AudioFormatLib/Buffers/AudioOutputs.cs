using AudioFormatLib.IO;

namespace AudioFormatLib.Buffers;

internal class AudioOutputs : IAudioOutputs
{
    public AFrameFormat Format { get { return _format; } }

    public IAudioBufferOutput Buffer { get { return _bufferOutput; } }

    public IAudioStreamOutput Stream { get { return _streamOutput; } }



    private readonly AFrameFormat _format;

    private readonly IAudioBufferOutput _bufferOutput;

    private readonly IAudioStreamOutput _streamOutput;

    internal AudioOutputs(ABufferParams bparams, IUnsafeBuffer buffer)
    {
        _format = bparams.Format;
        _bufferOutput = new SharedAudioBufferOutput(bparams.Format, buffer);
        _streamOutput = (IAudioStreamOutput)_bufferOutput;
    }
}
