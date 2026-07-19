using AudioFormatLib.Buffers;
using AudioFormatLib.Extensions;
using AudioFormatLib.Resampler;

namespace AudioFormatLib.Utils;


public class AudioResampler 
    : AudioFunction<bool, long>
{
    private const int FinalFlushSampleValueCapacityPerChannel = 4_096;

    public static AudioResampler CreatePcm16(
        int inputSampleRate,
        int outputSampleRate,
        int channelCount = 1,
        bool highQuality = true)
    {
        if (inputSampleRate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(inputSampleRate));
        }

        if (outputSampleRate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(outputSampleRate));
        }

        if (channelCount is < 1 or > 2)
        {
            throw new ArgumentOutOfRangeException(nameof(channelCount));
        }

        return Create(new AResamplerParams
        {
            HighQuality = highQuality,
            Input = new APcmFormat(ASampleValueFormat.S16, inputSampleRate, channelCount),
            Output = new APcmFormat(ASampleValueFormat.S16, outputSampleRate, channelCount)
        });
    }

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
        if (!_coupler.AttachBuffers(input, output))
        {
            throw new InvalidOperationException("The resampler is already processing another buffer.");
        }

        try
        {
            return Process(Params);
        }
        finally
        {
            _coupler.ReleaseBuffers();
        }
    }

    public T[] Process<T>(T[] input)
        where T : unmanaged
    {
        return Process(input, endOfInput: false);
    }

    public T[] Process<T>(T[] input, bool endOfInput)
        where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(input);
        return Process(input, 0, input.LongLength, endOfInput);
    }

    public T[] Process<T>(T[] input, long sampleValueOffset, long sampleValueCount)
        where T : unmanaged
    {
        return Process(input, sampleValueOffset, sampleValueCount, endOfInput: false);
    }

    public T[] Process<T>(
        T[] input,
        long sampleValueOffset,
        long sampleValueCount,
        bool endOfInput)
        where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(input);
        if (sampleValueOffset < 0 || sampleValueCount < 0 ||
            sampleValueOffset > input.LongLength - sampleValueCount)
        {
            throw new ArgumentOutOfRangeException(nameof(sampleValueOffset));
        }

        if (!Params.Input.SampleValueFormat.IsCompatible<T>() ||
            !Params.Output.SampleValueFormat.IsCompatible<T>())
        {
            throw new ArgumentException("The array element type does not match the resampler PCM format.", nameof(input));
        }

        long outputCapacity = Params.GetExpectedOutputSampleValueCapacity(sampleValueCount);
        if (endOfInput)
        {
            outputCapacity = checked(
                outputCapacity + (long)FinalFlushSampleValueCapacityPerChannel * Params.ChannelCount);
        }

        if (outputCapacity > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(sampleValueCount), "The output array would be too large.");
        }

        T[] output = new T[outputCapacity];
        long outputSampleValueCount = Process<T, T>(
            input,
            sampleValueOffset,
            sampleValueCount,
            output,
            0,
            outputCapacity,
            endOfInput);
        if (outputSampleValueCount < 0 || outputSampleValueCount > outputCapacity)
        {
            throw new InvalidOperationException("The resampler returned an invalid output sample-value count.");
        }

        Array.Resize(ref output, (int)outputSampleValueCount);
        return output;
    }
}
