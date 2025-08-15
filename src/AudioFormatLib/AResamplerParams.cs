using AudioFormatLib.Utils;

namespace AudioFormatLib;


/// <summary>
/// 
/// Parameters:
/// <list type="bullet">
///     <item>highQuality - If false it will use lower quality algorithm, but faster.</item>
///     <item>factor - Ratio between input and output sample rate. If it is zero then both input and output sample rate parameters must be provided.</item>
///     <item>inputSampleRate - Can be zero if parameter 'factor' is non-zero value.</item>
///     <item>outputSampleRate - Can be zero if parameter 'factor' is non-zero value.</item>
///     <item>numChannels - Number of audio channels in input stream. Supported values aree 1 and 2.</item>
///     <item>sampleFormat - Input sample format. Supported value is <see cref="ASampleFormat.S16"/>, signed 16-bit integer.</item>
///     <item>outSampleFormat - Default is 0, the same as input sample format.</item>
///     <item>outChannels - Default is 0, the same as number of channels at input.</item>
/// </list>
/// FYI: Some of input/output conversions are still work in progress.
/// </summary>
public struct AResamplerParams
{
    public bool HighQuality;

    public float Factor;

    public int InputSampleRate;

    public int OutputSampleRate;

    public int NumChannels;

    public ASampleFormat SampleFormat;

    public ASampleFormat OutSampleFormat;

    public int OutChannels;

    public AResamplerParams(bool highQuality,
                           float factor,
                           int inputSampleRate = 0,
                           int outputSampleRate = 0,
                           int numChannels = 1,
                           ASampleFormat sampleFormat = ASampleFormat.S16,
                           ASampleFormat outSampleFormat = ASampleFormat.NONE,
                           int outChannels = 0)
    {
        if (numChannels < 1 || numChannels > 2 || outChannels < 0 || outChannels > 2)
        {
            throw new ArgumentException("Unsupported number of channels.");
        }

        if (sampleFormat != ASampleFormat.S16)
        {
            throw new ArgumentException("Unsupported sample format.");
        }

        HighQuality = highQuality;
        Factor = AudioFrameTools.CalcFactor(factor, inputSampleRate, outputSampleRate);
        InputSampleRate = AudioFrameTools.CalcInputSampleRate(factor, inputSampleRate, outputSampleRate);
        OutputSampleRate = AudioFrameTools.CalcOutputSampleRate(factor, inputSampleRate, outputSampleRate);
        NumChannels = numChannels;
        SampleFormat = sampleFormat;
        OutSampleFormat = (outSampleFormat == ASampleFormat.NONE) ? sampleFormat : outSampleFormat;
        OutChannels = outChannels;
    }

    /// <summary>
    /// 
    /// As per function name, calculate expected output size for given input size and resampling factor.
    /// 
    /// </summary>
    /// <param name="inputSize"></param>
    /// <param name="factor"></param>
    /// <returns></returns>
    public long GetExpectedOutputSize(long inputSize)
    {
        return (long)((double)inputSize * Factor) + 400;
    }
}
