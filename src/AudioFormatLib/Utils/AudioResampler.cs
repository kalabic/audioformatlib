using AudioFormatLib.Extensions;
using AudioFormatLib.Resampler;

namespace AudioFormatLib.Utils;


public unsafe class AudioResampler 
    : AudioFunction<bool, long>
{
    public static AudioResampler Create(AResamplerParams RP)
    {
        if (RP.NumChannels < 1 || RP.NumChannels > 2 || RP.OutChannels < 0 || RP.OutChannels > 2)
        {
            throw new ArgumentException("Unsupported number of channels.");
        }
        if (RP.SampleFormat != ASampleFormat.S16)
        {
            throw new ArgumentException("Unsupported sample format.");
        }
        if (RP.Factor == 0.0f)
        {
            RP.Factor = AudioFrameTools.CalcFactor(RP.Factor, RP.InputSampleRate, RP.OutputSampleRate);
        }
        if (RP.InputSampleRate == 0)
        {
            RP.InputSampleRate = AudioFrameTools.CalcInputSampleRate(RP.Factor, RP.InputSampleRate, RP.OutputSampleRate);
        }
        if (RP.OutputSampleRate == 0)
        {
            RP.OutputSampleRate = AudioFrameTools.CalcOutputSampleRate(RP.Factor, RP.InputSampleRate, RP.OutputSampleRate);
        }
        RP.OutSampleFormat = (RP.OutSampleFormat == ASampleFormat.NONE) ? RP.SampleFormat : RP.OutSampleFormat;
        return new AudioResampler(RP);
    }

    public AResamplerParams Params { get { return _RP; } }

    public long InPacketCount { get { return _inPacketCount; } }

    public long InBytesProcessed { get { return _inBytesProcessed; } }

    public long OutBytesGenerated { get { return _outBytesGenerated; } }


    private readonly AResamplerParams _RP;

    private ChannelResampler[] _resamplers;

    private long _inPacketCount = 0;

    private long _inBytesProcessed = 0;

    private long _outBytesGenerated = 0;

    protected AudioResampler(AResamplerParams RP)
        : base(RP.NumChannels, RP.SampleFormat.Size(), RP.OutSampleFormat.Size())
    {
        _RP = RP;
        _resamplers = new ChannelResampler[RP.NumChannels];
        for (int i = 0; i < RP.NumChannels; i++)
        {
            _resamplers[i] = new ChannelResampler(RP.HighQuality, RP.Factor);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _resamplers = Array.Empty<ChannelResampler>();
        }
        base.Dispose(disposing);
    }

    protected unsafe override long Process(bool lastFrame)
    {
        long samplesWritten = 0;
        long samplesRead = 0;
        for (int i=0; i < _RP.NumChannels; i++)
        {
            var producer = GetProducer(i);
            var consumer = GetConsumer(i);
            _resamplers[i].ProcessInput(_RP.Factor, lastFrame, producer, consumer);
            samplesRead += producer.SamplesRead;
            samplesWritten += consumer.SamplesWritten;
        }

        _inPacketCount++;
        _inBytesProcessed += samplesRead * _RP.SampleFormat.Size();
        _outBytesGenerated += samplesWritten * _RP.OutSampleFormat.Size();
        return samplesWritten;
    }

    protected override long IOErrorOccurred()
    {
        return 0;
    }

    protected override long ExceptionOccurred(Exception ex)
    {
        return 0;
    }
}
