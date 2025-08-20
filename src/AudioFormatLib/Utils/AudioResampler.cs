using AudioFormatLib.Buffers;
using AudioFormatLib.Extensions;
using AudioFormatLib.Resampler;

namespace AudioFormatLib.Utils;


public class AudioResampler 
    : AudioFunction<bool, long>
{
    public static AudioResampler Create(AResamplerParams RP)
    {
        if (RP.Input.NumChannels < 1 || RP.Input.NumChannels > 2 || RP.Output.NumChannels < 0 || RP.Output.NumChannels > 2)
        {
            throw new ArgumentException("Unsupported number of channels.");
        }
        if (RP.Input.SampleFormat != ASampleFormat.S16)
        {
            throw new ArgumentException("Unsupported sample format.");
        }
        if (RP.Factor == 0.0f)
        {
            RP.Factor = ATools.CalcFactor(RP.Factor, RP.Input.SampleRate, RP.Output.SampleRate);
        }
        if (RP.Input.SampleRate == 0)
        {
            RP.Input.SampleRate = ATools.CalcInputSampleRate(RP.Factor, RP.Input.SampleRate, RP.Output.SampleRate);
        }
        if (RP.Output.SampleRate == 0)
        {
            RP.Output.SampleRate = ATools.CalcOutputSampleRate(RP.Factor, RP.Input.SampleRate, RP.Output.SampleRate);
        }
        RP.Output = (RP.Output.SampleFormat == ASampleFormat.NONE) ? RP.Input : RP.Output;
        return new AudioResampler(RP);
    }



    public AResamplerParams Params { get { return _RP; } }

    public long InPacketCount { get { return _inPacketCount; } }

    public long InBytesProcessed { get { return _inBytesProcessed; } }

    public long OutBytesGenerated { get { return _outBytesGenerated; } }



    private readonly AResamplerParams _RP;

    private ChannelResampler[] _resamplers;

    private readonly BufferCoupler _coupler;

    private long _inPacketCount = 0;

    private long _inBytesProcessed = 0;

    private long _outBytesGenerated = 0;



    protected AudioResampler(in AResamplerParams RP)
        : base(RP.Input, RP.Output)
    {
        _RP = RP;
        _resamplers = new ChannelResampler[RP.NumChannels];
        for (int i = 0; i < RP.NumChannels; i++)
        {
            _resamplers[i] = new ChannelResampler(RP.HighQuality, RP.Factor);
        }
        _coupler = new BufferCoupler(RP.Input, RP.Output);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _resamplers = Array.Empty<ChannelResampler>();
            _coupler.Dispose();
        }
        base.Dispose(disposing);
    }

    protected long Process(bool lastFrame)
    {
        long samplesWritten = 0;
        long samplesRead = 0;
        for (int i=0; i < _RP.NumChannels; i++)
        {
            var producer = _coupler.GetProducer(i);
            var consumer = _coupler.GetConsumer(i);
            _resamplers[i].ProcessInput(_RP.Factor, lastFrame, producer, consumer);
            samplesRead += producer.SamplesRead;
            samplesWritten += consumer.SamplesWritten;
        }

        _inPacketCount++;
        _inBytesProcessed += samplesRead * _RP.Input.SampleFormat.Size();
        _outBytesGenerated += samplesWritten * _RP.Output.SampleFormat.Size();
        return samplesWritten;
    }

    /// <summary>
    /// 
    /// Process input/output provided as AudioSpan types. All other pointer and Array combinations of 
    /// input and output types also invoke this method.
    /// 
    /// </summary>
    public override long Process(in AudioSpan input, in AudioSpan output, bool Params)
    {
        if (_coupler.AttachBuffers(input, output))
        {
            try
            {
                return Process(Params);
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return 0;
            }
            finally
            {
                _coupler.ReleaseBuffers();
            }
        }
        else
        {
            return 0;
        }
    }

    public T[] Process<T>(T[] input)
        where T : unmanaged
    {
        return Process(input, 0, input.LongLength);
    }

    public T[] Process<T>(T[] input, long offset, long length)
        where T : unmanaged
    {
        long expectedSize = (int)Params.GetExpectedOutputSize(length);
        T[] output = new T[expectedSize];
        long outputSize = Process<T, T>(input, offset, length, output, 0, expectedSize, false);
        Array.Resize(ref output, (int)outputSize);
        return output;
    }
}
