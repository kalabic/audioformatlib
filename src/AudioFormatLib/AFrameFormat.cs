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
    /// <summary> 
    ///
    /// Notice that "frame sample" represents samples across all channels for a single timestamp.
    /// Just to be clear here, because the term "sample" is overloaded in audio terminology.
    ///
    /// </summary>
    public int FrameSampleSize { get { return SampleFormat.Size() * ChannelFormat.Count; } }

    public ASampleFormat SampleFormat;

    public AChannelFormat ChannelFormat;

    public int SampleRate;


    public AFrameFormat(ASampleFormat format, int sampleRate, int numChannels)
    {
        SampleFormat = format;
        SampleRate = sampleRate;
        ChannelFormat = new AChannelFormat(numChannels);
    }

    public long SampleCountFromBufferSize(long size)
    {
        Debug.Assert(size % FrameSampleSize == 0);
        return size / FrameSampleSize;
    }

    public long BufferSizeFromSeconds(long seconds)
    {
        return FrameSampleSize * SampleRate * seconds;
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
        long buf1000 = BufferSizeFromSeconds(miliseconds);
        return buf1000 / 1000;
    }
}
