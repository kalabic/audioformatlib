using AudioFormatLib.IO;
using System;

namespace AudioFormatLib.Buffers;

internal class AudioInputs : IAudioInputs
{
    public AFrameFormat Format { get { return _format; } }

    public IAudioBufferInput Buffer { get { return _bufferInput; } }

    public IAudioStreamInput Stream { get { return _streamInput; } }



    private readonly AFrameFormat _format;

    private readonly IAudioBufferInput _bufferInput;

    private readonly IAudioStreamInput _streamInput;

    internal AudioInputs(ABufferParams bparams, IUnsafeBuffer buffer)
    {
        _format = bparams.Format;
        _bufferInput = new SharedAudioBufferInput(bparams.Format, buffer);
        _streamInput = new SharedAudioStreamInput(bparams.Format, buffer);
    }
}
