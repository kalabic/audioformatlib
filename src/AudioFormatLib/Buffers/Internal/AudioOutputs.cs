using AudioFormatLib.IO;
using DotBase.Buffers.Integral;

namespace AudioFormatLib.Buffers.Internal;


internal class AudioOutputs : IAudioOutputs
{
    public APcmFormat Format { get { return _format; } }

    public IAudioBufferOutput Buffer { get { return _bufferOutput; } }

    public IPcm16ValueOutput? Pcm16Values { get { return _pcm16ValueOutput; } }

    public IPcm16FrameOutput? Pcm16Frames { get { return _pcm16FrameOutput; } }

    public IAudioStreamOutput Stream { get { return _streamOutput; } }



    private readonly APcmFormat _format;

    private readonly IAudioBufferOutput _bufferOutput;

    private readonly IPcm16ValueOutput? _pcm16ValueOutput;

    private readonly IPcm16FrameOutput? _pcm16FrameOutput;

    private readonly IAudioStreamOutput _streamOutput;

    internal AudioOutputs(ABufferParams bparams, IIntegralRingBuffer buffer)
    {
        _format = bparams.Format;
        _pcm16ValueOutput = SharedPcm16View.IsValueCompatible(bparams.Format)
            ? new SharedPcm16ValueOutput(bparams.Format, buffer)
            : null;
        _pcm16FrameOutput = SharedPcm16View.IsFrameCompatible(bparams.Format)
            ? new SharedPcm16FrameOutput(bparams.Format, buffer)
            : null;
        _bufferOutput = new SharedAudioBufferOutput(
            bparams.Format,
            buffer,
            _pcm16ValueOutput);
        _streamOutput = (IAudioStreamOutput)_bufferOutput;
    }

}
