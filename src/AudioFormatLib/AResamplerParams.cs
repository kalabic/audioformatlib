using System.ComponentModel;

namespace AudioFormatLib;


/// <summary>
/// 
/// Parameters:
/// <list type="bullet">
///     <item>HighQuality - If false it will use lower quality algorithm, but faster.</item>
///     <item>Factor - Ratio between input and output sample rate. If it is zero then both input and output sample rate parameters must be provided.</item>
///     <item>InputSampleRate - Can be zero if parameter 'Factor' is non-zero value.</item>
///     <item>OutputSampleRate - Can be zero if parameter 'Factor' is non-zero value.</item>
///     <item>NumChannels - Number of audio channels in input stream. Supported values aree 1 and 2.</item>
///     <item>SampleFormat - Input sample format. Supported value is <see cref="ASampleFormat.S16"/>, signed 16-bit integer.</item>
///     <item>OutSampleFormat - Default is 0, the same as input sample format.</item>
///     <item>OutChannels - Default is 0, the same as number of channels at input.</item>
/// </list>
/// FYI: Some of input/output conversions are still work in progress.
/// </summary>
public struct AResamplerParams
{
    [DefaultValue(false)]
    public bool HighQuality { get; set; } = false;


    [DefaultValue(0.0f)]
    public float Factor { get; set; } = 0.0f;


    [DefaultValue(0)]
    public int InputSampleRate { get; set; } = 0;


    [DefaultValue(0)]
    public int OutputSampleRate { get; set; } = 0;


    [DefaultValue(1)]
    public int NumChannels { get; set; } = 1;


    [DefaultValue(ASampleFormat.S16)]
    public ASampleFormat SampleFormat { get; set; } = ASampleFormat.S16;


    [DefaultValue(ASampleFormat.NONE)]
    public ASampleFormat OutSampleFormat { get; set; } = ASampleFormat.NONE;


    [DefaultValue(1)]
    public int OutChannels { get; set; } = 1;


    public AResamplerParams() { }

    /// <summary>
    /// 
    /// As per function name, calculate expected output size for given input size and resampling Factor.
    /// 
    /// </summary>
    /// <param name="inputSize"></param>
    /// <param name="factor"></param>
    /// <returns></returns>
    public long GetExpectedOutputSize(long inputSize)
    {
        long size = (long)Math.Ceiling((double)inputSize * Factor);
        // Append some to output, but not too much.
        long append = (size >= 512) ? Math.Min(128, size/32) : Math.Min(16, size / 32 + 4);
        return size + append;
    }
}
