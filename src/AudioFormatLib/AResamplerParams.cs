using System.ComponentModel;
using System.Diagnostics;

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
///     <item>Input - Input sample format. Supported value is <see cref="ASampleFormat.S16"/>, signed 16-bit integer.</item>
///     <item>Output - Default is 0, the same as input sample format.</item>
///     <item>OutChannels - Default is 0, the same as number of channels at input.</item>
/// </list>
/// FYI: Some of input/output conversions are still work in progress.
/// </summary>
public struct AResamplerParams
{
    public bool HighQuality { get; set; } = false;

    public float Factor { get; set; } = 0.0f;

    public int NumChannels { get { return Input.NumChannels; } }


    public AFrameFormat Input = AFrameFormat.NONE;

    public AFrameFormat Output = AFrameFormat.NONE;


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
        long append = (size >= 512) ? Math.Min(128, size / 32) : Math.Min(16, size / 32 + 4);
        var result = size + append;

        if ((result % 16) != 0)
        {
            result |= 0x0F;
            result++;
            Debug.Assert((result % 16) == 0);
        }

        return result;
    }
}
