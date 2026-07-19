/******************************************************************************
 *
 * This file is part of AudioFormatLib, which is free software:
 * you can redistribute it and/or modify it under the terms of the
 * GNU Lesser General Public License (LGPL), version 2.1 only,
 * as published by the Free Software Foundation.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * version 2.1 along with this file. If not, see:
 * https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html 
 *
 *****************************************************************************/

using System.Diagnostics;

namespace AudioFormatLib;


/// <summary>
/// 
/// Describes the value format, sampling rate, channel layout, and storage layout of PCM audio.
/// A sample contains one sample value from every channel at a single timestamp.
/// 
/// </summary>
public struct APcmFormat
{
    public static readonly APcmFormat NONE = new APcmFormat(ASampleValueFormat.NONE, 0, 0);

    /// <summary> Number of bytes occupied by one sample across all channels. </summary>
    public int BytesPerSampleFrame { get { return SampleValueFormat.Size() * ChannelLayout.Count; } }

    public int ChannelCount { get { return ChannelLayout.Count; } }

    public bool IsPlanar {  get { return ChannelLayout.IsPlanar; } }


    public ASampleValueFormat SampleValueFormat;

    public AChannelLayout ChannelLayout;

    public int SampleRate;


    /// <summary>
    /// 
    /// Note that if <paramref name="channelCount"/> is 1, it overrides value of <paramref name="planar"/> and
    /// always sets it to <c>true</c>.
    /// 
    /// </summary>
    /// <param name="sampleValueFormat"></param>
    /// <param name="sampleRate"></param>
    /// <param name="channelCount"></param>
    /// <param name="planar"></param>
    public APcmFormat(
        ASampleValueFormat sampleValueFormat,
        int sampleRate,
        int channelCount,
        bool planar = false)
    {
        SampleValueFormat = sampleValueFormat;
        SampleRate = sampleRate;
        ChannelLayout = new AChannelLayout(channelCount, planar);
    }

    /// <summary> Convert a count of scalar sample values to bytes. </summary>
    public long AsByteCount(long sampleValueCount)
    {
        return sampleValueCount * SampleValueFormat.Size();
    }

    /// <summary> Convert bytes to a count of scalar sample values. </summary>
    public long AsSampleValueCount(long byteCount)
    {
        Debug.Assert(byteCount % SampleValueFormat.Size() == 0);
        return byteCount / SampleValueFormat.Size();
    }

    /// <summary> Convert scalar sample values to temporal samples. </summary>
    public long AsSampleCount(long sampleValueCount)
    {
        Debug.Assert(sampleValueCount % ChannelLayout.Count == 0);
        return sampleValueCount / ChannelLayout.Count;
    }

    public long SampleCountFromBufferSize(long byteCount)
    {
        Debug.Assert(byteCount % BytesPerSampleFrame == 0);
        return byteCount / BytesPerSampleFrame;
    }

    public long BufferSizeFromSeconds(long seconds)
    {
        return BytesPerSampleFrame * SampleRate * seconds;
    }

    /// <summary>
    /// 
    /// Notice here (if you care) that in most cases 'SampleRate * milliseconds' needs
    /// to give number divisible by 1000 for the math to be absolutely correct.
    /// 
    /// </summary>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public long BufferSizeFromMiliseconds(long milliseconds)
    {
        return (BytesPerSampleFrame * SampleRate * milliseconds) / 1000;
    }
}
