using System.Diagnostics;

namespace AudioFormatLib;


public readonly struct ACountOf
{
    /// <summary> Must be multiple of a FRAME size. </summary>
    public readonly long Bytes;

    /// <summary> FRAME size, <see cref="ACountOf.Bytes"/> property must be multiple of it. </summary>
    public int BytesInFrame { get { return BytesInSample * Channels; } }

    /// <summary> SAMPLE size, <see cref="ACountOf.BytesInFrame"/> and <see cref="ACountOf.Bytes"/>  properties must be multiple of it. </summary>
    public readonly int BytesInSample;

    /// <summary>
    /// 
    /// Audio FRAME represents time-aligned group of SAMPLES across channels.  So in case of
    /// multi-channel audio that makes FRAME COUNT equivalent to count of samples inside ONE
    /// of channels.
    /// 
    /// </summary>
    public readonly long Frames;

    /// <summary> Count of all samples across all channels. </summary>
    public readonly long Samples;

    /// <summary> Number of SAMPLES in a FRAME. </summary>
    public readonly int Channels;

    public ACountOf(long byteLength, ASampleFormat fmt, int numChannels)
    {
        Debug.Assert(numChannels >= 1);
        Debug.Assert(fmt != ASampleFormat.NONE);
        var sampleSize = fmt.Size();
        var frameSize = sampleSize * numChannels;
        Debug.Assert((byteLength % frameSize) == 0, "Incomplete audio frame.");

        // TODO: Still undecided what to do when asserts fail.

        Bytes = byteLength;
        BytesInSample = sampleSize;
        Channels = numChannels;
        Samples = byteLength / fmt.Size();
        Frames = Samples / numChannels;
    }
}
