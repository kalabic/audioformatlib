using AudioFormatLib.Buffers;
using AudioFormatLib.Resampler;

namespace AudioFormatLib.Utils;


public unsafe partial class AudioFrameResampler : DisposableBuffer
{
    public AResamplerParams Params { get { return _params; } }

    public long InPacketCount { get { return _inPacketCount; } }

    public long InBytesProcessed { get { return _inBytesProcessed; } }

    public long OutBytesGenerated { get { return _outBytesGenerated; } }


    private AResamplerParams _params;

    private readonly BufferCoupler _coupler;

    private ChannelResampler[] _resamplers;

    private long _inPacketCount = 0;

    private long _inBytesProcessed = 0;

    private long _outBytesGenerated = 0;

    public AudioFrameResampler(AResamplerParams parameters)
    {
        _params = parameters;
        _coupler = new BufferCoupler(_params.NumChannels, _params.SampleFormat.Size(), _params.OutSampleFormat.Size());

        _resamplers = new ChannelResampler[_params.NumChannels];
        for (int i = 0; i < _params.NumChannels; i++)
        {
            _resamplers[i] = new ChannelResampler(_params.HighQuality, _params.Factor);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _coupler.Dispose();
            _resamplers = Array.Empty<ChannelResampler>();
        }
        base.Dispose(disposing);
    }

    public unsafe long Resample(bool lastPacket)
    {
        _coupler.EnsureAttached();

        long samplesWritten = 0;
        long samplesRead = 0;
        for (int i=0; i < _params.NumChannels; i++)
        {
            var producer = _coupler.GetProducer(i);
            var consumer = _coupler.GetConsumer(i);
            _resamplers[i].ProcessInput(_params.Factor, lastPacket, producer, consumer);
            samplesRead += producer.SamplesRead;
            samplesWritten += consumer.SamplesWritten;
        }

        _inPacketCount++;
        _inBytesProcessed += samplesRead * _params.SampleFormat.Size();
        _outBytesGenerated += samplesWritten * _params.OutSampleFormat.Size();

        _coupler.ReleaseBuffers();
        return samplesWritten;
    }

    public unsafe long Resample<IN, OUT>(bool  lastPacket,
                                         IN[]  input,
                                         long  inputLength,
                                         OUT[] output,
                                         long  outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        _coupler.AttachBuffers(input, inputLength, output, outputLength);
        return Resample(lastPacket);
    }

    public unsafe long Resample<IN, OUT>(bool  lastPacket,
                                         IN*   input,
                                         long  inputLength,
                                         OUT[] output,
                                         long  outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        _coupler.AttachBuffers(input, inputLength, output, outputLength);
        return Resample(lastPacket);
    }

    public unsafe long Resample<IN, OUT>(bool lastPacket,
                                         IN[] input,
                                         long inputLength,
                                         OUT* output,
                                         long outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        _coupler.AttachBuffers(input, inputLength, output, outputLength);
        return Resample(lastPacket);
    }

    public unsafe long Resample<IN, OUT>(bool lastPacket,
                                         IN*  input, 
                                         long inputLength, 
                                         OUT* output, 
                                         long outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        _coupler.AttachBuffers(input, inputLength, output, outputLength);
        return Resample(lastPacket);
    }
}
