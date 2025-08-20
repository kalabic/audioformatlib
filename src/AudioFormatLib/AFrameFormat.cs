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
/// Audio frame represents time-aligned group of samples across all channels inside an audio packet.
/// 
/// </summary>
public struct AFrameFormat
{
    public static readonly AFrameFormat NONE = new AFrameFormat(ASampleFormat.NONE, 0, 0);

    /// <summary> FRAME represents samples across all channels for a single timestamp. </summary>
    public int FrameSize { get { return SampleFormat.Size() * ChannelLayout.Count; } }

    public int NumChannels { get { return ChannelLayout.Count; } }

    public bool IsPlanar {  get { return ChannelLayout.IsPlanar; } }


    public ASampleFormat SampleFormat;

    public AChannelLayout ChannelLayout;

    public int SampleRate;


    /// <summary>
    /// 
    /// Note that if <paramref name="numChannels"/> is 1, it overrides value of <paramref name="planar"/> and
    /// always sets it to <c>true</c>.
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="sampleRate"></param>
    /// <param name="numChannels"></param>
    /// <param name="planar"></param>
    public AFrameFormat(ASampleFormat format, int sampleRate, int numChannels, bool planar = false)
    {
        SampleFormat = format;
        SampleRate = sampleRate;
        ChannelLayout = new AChannelLayout(numChannels, planar);
    }

    /// <summary> Convert samples to bytes. </summary>
    public long AsByteCount(long sampleCount)
    {
        return sampleCount * SampleFormat.Size();
    }

    /// <summary> Convert bytes to samples. </summary>
    public long AsSampleCount(long byteCount)
    {
        Debug.Assert(byteCount % SampleFormat.Size() == 0);
        return byteCount / SampleFormat.Size();
    }

    public long AsFrameCount(long sampleCount)
    {
        Debug.Assert(sampleCount % ChannelLayout.Count == 0);
        return sampleCount / ChannelLayout.Count;
    }

    public long SampleCountFromBufferSize(long size)
    {
        Debug.Assert(size % FrameSize == 0);
        return size / FrameSize;
    }

    public long BufferSizeFromSeconds(long seconds)
    {
        return FrameSize * SampleRate * seconds;
    }

    /// <summary>
    /// 
    /// Notice here (if you care) that in most cases 'SampleRate * miliseconds' needs
    /// to give number divisible by 1000 for the math to be absolutely correct.
    /// 
    /// </summary>
    /// <param name="miliseconds"></param>
    /// <returns></returns>
    public long BufferSizeFromMiliseconds(long miliseconds)
    {
        return (FrameSize * SampleRate * miliseconds) / 1000;
    }
}
