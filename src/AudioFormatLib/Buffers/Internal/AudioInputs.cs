using AudioFormatLib.IO;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal class AudioInputs : IAudioInputs
{
    public APcmFormat Format { get { return _format; } }

    public IAudioBufferInput Buffer { get { return _bufferInput; } }

    public IPcm16ValueInput? Pcm16Values { get { return _pcm16ValueInput; } }

    public IPcm16FrameInput? Pcm16Frames { get { return _pcm16FrameInput; } }

    public IAudioStreamInput Stream { get { return _streamInput; } }



    private readonly APcmFormat _format;

    private readonly IAudioBufferInput _bufferInput;

    private readonly IPcm16ValueInput? _pcm16ValueInput;

    private readonly IPcm16FrameInput? _pcm16FrameInput;

    private readonly IAudioStreamInput _streamInput;

    internal AudioInputs(ABufferParams bparams, IIntegralRingBuffer buffer)
    {
        _format = bparams.Format;
        _pcm16ValueInput = SharedPcm16View.IsValueCompatible(bparams.Format)
            ? new SharedPcm16ValueInput(bparams.Format, buffer)
            : null;
        _pcm16FrameInput = SharedPcm16View.IsFrameCompatible(bparams.Format)
            ? new SharedPcm16FrameInput(bparams.Format, buffer)
            : null;
        _bufferInput = new SharedAudioBufferInput(
            bparams.Format,
            buffer,
            _pcm16ValueInput);
        _streamInput = new SharedAudioStreamInput(bparams.Format, buffer);
    }

}
