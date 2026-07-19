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
///     <item>ChannelCount - Number of audio channels in input stream. Supported values aree 1 and 2.</item>
///     <item>Input - Input PCM format. Signed 16-bit sample values are supported.</item>
///     <item>Output - Output PCM format. The default is the input format.</item>
///     <item>OutChannels - Default is 0, the same as number of channels at input.</item>
/// </list>
/// FYI: Some of input/output conversions are still work in progress.
/// </summary>
public struct AResamplerParams
{
    public bool HighQuality { get; set; } = false;

    public float Factor { get; set; } = 0.0f;

    public int ChannelCount { get { return Input.ChannelCount; } }


    public APcmFormat Input = APcmFormat.NONE;

    public APcmFormat Output = APcmFormat.NONE;


    public AResamplerParams() { }

    /// <summary>
    /// 
    /// Calculate output sample-value capacity for a supplied input sample-value count.
    /// 
    /// </summary>
    /// <param name="inputSampleValueCount">Total scalar sample values across all channels.</param>
    public long GetExpectedOutputSampleValueCapacity(long inputSampleValueCount)
    {
        long size = (long)Math.Ceiling((double)inputSampleValueCount * Factor);
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
