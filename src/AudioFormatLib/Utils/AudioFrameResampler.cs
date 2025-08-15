using AudioFormatLib.IO;
using AudioFormatLib.Resampler;

namespace AudioFormatLib.Utils;


/// <summary>
/// Use one of following static members to create an instance of resampler:
/// <list type="bullet">
///     <item> To work with unsafe memory buffers: <see cref="AudioFrameResampler.New_BytePtr_Resampler"/> </item>
///     <item> To work with managed byte[] frames: <see cref="AudioFrameResampler.New_ByteFrame_Resampler"/> </item>
///     <item> To work with managed short[] frames: <see cref="AudioFrameResampler.New_ShortFrame_Resampler"/> </item>
///     <item> To work with managed float[] frames: <see cref="AudioFrameResampler.New_FloatFrame_Resampler"/> </item>
/// </list>
/// </summary>
public unsafe class AudioFrameResampler 
    : IResamplerFunction_BytePtr
    , IResamplerFunction<byte[], byte[]>
    , IResamplerFunction<short[], short[]>
    , IResamplerFunction<float[], float[]>
{
    /// <summary>
    /// 
    /// Create instance of resampler and return its <see cref="IResamplerFunction_BytePtr"/> interface.
    /// 
    /// <para>See summary for <see cref="AudioFrameResampler"/> class for description of parameters.</para>
    /// </summary>
    public static IResamplerFunction_BytePtr New_BytePtr_Resampler(AResamplerParams parameters)
    {
        return new AudioFrameResampler(parameters);
    }


    /// <summary>
    /// 
    /// Create instance of resampler and return its <see cref="IResamplerFunction{IN, OUT}"/> interface.
    /// 
    /// <para>See summary for <see cref="AudioFrameResampler"/> class for description of parameters.</para>
    /// </summary>
    public static IResamplerFunction<byte[], byte[]> New_ByteFrame_Resampler(AResamplerParams parameters)
    {
        return new AudioFrameResampler(parameters);
    }


    /// <summary>
    /// 
    /// Create instance of resampler and return its <see cref="IResamplerFunction{IN, OUT}"/> interface.
    /// 
    /// <para>See summary for <see cref="AudioFrameResampler"/> class for description of parameters.</para>
    /// </summary>
    public static IResamplerFunction<short[], short[]> New_ShortFrame_Resampler(AResamplerParams parameters)
    {
        return new AudioFrameResampler(parameters);
    }


    /// <summary>
    /// 
    /// Create instance of resampler and return its <see cref="IResamplerFunction{IN, OUT}"/> interface.
    /// 
    /// <para>See summary for <see cref="AudioFrameResampler"/> class for description of parameters.</para>
    /// </summary>
    public static IResamplerFunction<float[], float[]> New_FloatFrame_Resampler(AResamplerParams parameters)
    {
        return new AudioFrameResampler(parameters);
    }


    public AResamplerParams Params { get { return _params; } }

    public long InPacketCount { get { return _inPacketCount; } }

    public long InBytesProcessed { get { return _inBytesProcessed; } }

    public long OutBytesGenerated { get { return _outBytesGenerated; } }

    private AResamplerParams _params;

    private ChannelResampler[] _chrs;

    private long _inPacketCount = 0;

    private long _inBytesProcessed = 0;

    private long _outBytesGenerated = 0;

    public AudioFrameResampler(AResamplerParams parameters)
    {
        _params = parameters;

        _chrs = new ChannelResampler[_params.NumChannels];
        if (_params.NumChannels == 1)
        {
            _chrs[0] = new ChannelResampler(_params.HighQuality, _params.Factor, AChannelId.MonoTrack);
        }
        else if (_params.NumChannels == 2)
        {
            _chrs[0] = new ChannelResampler(_params.HighQuality, _params.Factor, AChannelId.LeftStereo);
            _chrs[1] = new ChannelResampler(_params.HighQuality, _params.Factor, AChannelId.RightStereo);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < _params.NumChannels; i++)
        {
            _chrs[i].Dispose();
        }
        _params.NumChannels = 0;
    }

    public unsafe long Process(bool lastPacket, byte* input, long inputSize, byte* output, long outputSize)
    {
        long outputUsed = 0;
        for (int i=0; i < _params.NumChannels; i++)
        {
            // TODO(?): Assert that all channels returned the same value.
            outputUsed += _chrs[i].ProcessInput(
                _params.Factor, lastPacket, input, inputSize / _params.NumChannels, output, outputSize / _params.NumChannels);
        }

        _inPacketCount++;
        _inBytesProcessed += inputSize;
        _outBytesGenerated += outputSize;
        return outputUsed;
    }

    public unsafe long Process(bool lastPacket, byte[] input, long inputSize, byte[] output, long outputSize)
    {
        fixed (byte* inputPtr = input, outputPtr = output)
        {
            return Process(lastPacket, inputPtr, inputSize, outputPtr, outputSize);
        }
    }

    public long Process(bool lastPacket, short[] input, long inputSize, short[] output, long outputSize)
    {
        fixed (short* inputPtr = input, outputPtr = output)
        {
            return Process(lastPacket, (byte *)inputPtr, inputSize * 2, (byte *)outputPtr, outputSize * 2) / 2;
        }
    }

    public long Process(bool lastPacket, float[] input, long inputSize, float[] output, long outputSize)
    {
        throw new NotImplementedException();
    }
}
