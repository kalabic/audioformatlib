using System.Diagnostics;

namespace AudioFormatLib;


public struct AFAudioFormat
{
    /// <summary> Notice this value includes channel count. </summary>
    public int SampleSize { get { return SampleFormat.Bytes * Channel.Count; } }

    public AFSampleFormat SampleFormat;

    public int SampleRate;

    public AFChannelFormat Channel;


    public AFAudioFormat(AFFormatID format, int sampleRate, int numChannels)
    {
        SampleFormat = new AFSampleFormat(format);
        SampleRate = sampleRate;
        Channel = new AFChannelFormat(numChannels);
    }

    public long SampleCountFromBufferSize(long size)
    {
        Debug.Assert(size % SampleSize == 0);
        return size / SampleSize;
    }

    public long BufferSizeFromSeconds(long seconds)
    {
        return SampleSize * SampleRate * seconds;
    }

    /// <summary>
    /// Notice here (if you care) that 'SampleRate * miliseconds' needs to give number divisible by 1000 for the math to be absolutely correct.
    /// 
    /// </summary>
    /// <param name="miliseconds"></param>
    /// <returns></returns>
    public long BufferSizeFromMiliseconds(long miliseconds)
    {
        long buf1000 = BufferSizeFromSeconds(miliseconds);
        return buf1000 / 1000;
    }
}
