using System.Diagnostics;

namespace AudioFormatLib;


public readonly struct ACountOf
{
    /// <summary> Must be a multiple of one sample frame. </summary>
    public readonly long Bytes;

    /// <summary> Bytes occupied by one sample across all channels. </summary>
    public int BytesPerSampleFrame { get { return BytesPerSampleValue * ChannelCount; } }

    /// <summary> Bytes occupied by one scalar sample value. </summary>
    public readonly int BytesPerSampleValue;

    /// <summary> Temporal sample count; equivalently, the sample count in each channel. </summary>
    public readonly long Samples;

    /// <summary> Count of all scalar sample values across all channels. </summary>
    public readonly long SampleValues;

    /// <summary> Number of sample values in each sample. </summary>
    public readonly int ChannelCount;

    public ACountOf(long byteLength, ASampleValueFormat sampleValueFormat, int channelCount)
    {
        Debug.Assert(channelCount >= 1);
        Debug.Assert(sampleValueFormat != ASampleValueFormat.NONE);
        int sampleValueSize = sampleValueFormat.Size();
        int sampleFrameSize = sampleValueSize * channelCount;
        Debug.Assert((byteLength % sampleFrameSize) == 0, "Incomplete PCM sample.");

        // TODO: Still undecided what to do when asserts fail.

        Bytes = byteLength;
        BytesPerSampleValue = sampleValueSize;
        ChannelCount = channelCount;
        SampleValues = byteLength / sampleValueSize;
        Samples = SampleValues / channelCount;
    }
}
