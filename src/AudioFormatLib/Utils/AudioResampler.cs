using AudioFormatLib.Buffers;
using AudioFormatLib.Extensions;
using AudioFormatLib.Resampler;

namespace AudioFormatLib.Utils;


public class AudioResampler 
    : AudioFunction<bool, long>
{
    public static AudioResampler Create(AResamplerParams RP)
    {
        if (RP.Input.ChannelCount < 1 || RP.Input.ChannelCount > 2 || RP.Output.ChannelCount < 0 || RP.Output.ChannelCount > 2)
        {
            throw new ArgumentException("Unsupported number of channels.");
        }
        if (RP.Input.SampleValueFormat != ASampleValueFormat.S16)
        {
            throw new ArgumentException("Unsupported sample-value format.");
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
        RP.Output = (RP.Output.SampleValueFormat == ASampleValueFormat.NONE) ? RP.Input : RP.Output;
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
        _resamplers = new ChannelResampler[RP.ChannelCount];
        for (int i = 0; i < RP.ChannelCount; i++)
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

    protected long Process(bool endOfInput)
    {
        long sampleValuesWritten = 0;
        long sampleValuesRead = 0;
        for (int i=0; i < _RP.ChannelCount; i++)
        {
            var producer = _coupler.GetProducer(i);
            var consumer = _coupler.GetConsumer(i);
            _resamplers[i].ProcessInput(_RP.Factor, endOfInput, producer, consumer);
            sampleValuesRead += producer.SampleValuesRead;
            sampleValuesWritten += consumer.SampleValuesWritten;
        }

        _inPacketCount++;
        _inBytesProcessed += sampleValuesRead * _RP.Input.SampleValueFormat.Size();
        _outBytesGenerated += sampleValuesWritten * _RP.Output.SampleValueFormat.Size();
        return sampleValuesWritten;
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

    public T[] Process<T>(T[] input, long sampleValueOffset, long sampleValueCount)
        where T : unmanaged
    {
        long outputCapacity = Params.GetExpectedOutputSampleValueCapacity(sampleValueCount);
        T[] output = new T[outputCapacity];
        long outputSampleValueCount = Process<T, T>(
            input,
            sampleValueOffset,
            sampleValueCount,
            output,
            0,
            outputCapacity,
            false);
        Array.Resize(ref output, (int)outputSampleValueCount);
        return output;
    }
}
