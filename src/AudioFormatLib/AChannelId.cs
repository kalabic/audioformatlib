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
/// This structure identifies a single channel among others in a single or multi-channel audio frame.
/// 
/// </summary>
public struct AChannelId
{
    /// <summary> A channel Id that represents all tracks. </summary>
    public static readonly AChannelId EveryChannel = new AChannelId();

    /// <summary> For convenience when dealing with a mono audio frame. </summary>
    public static readonly AChannelId MonoTrack = new AChannelId(0);

    /// <summary> For convenience when dealing with a left channel of a stereo audio frame. </summary>
    public static readonly AChannelId LeftStereo = new AChannelId(0, 2);

    /// <summary> For convenience when dealing with a right channel of a stereo audio frame. </summary>
    public static readonly AChannelId RightStereo = new AChannelId(1, 2);


    /// <summary> Total count of channels in an audio frame. </summary>
    public int ChannelCount { get { return _channelCount; } }

    /// <summary> Index of a channel this instance identifies. </summary>
    public int Index { get { return _index; } }

    public bool AllChannels { get { return _channelCount == 0 && _index == 0; } }



    private int _channelCount;

    private int _index;


    /// <summary> Creates channel Id to be used when function needs to operate on each and every channel. </summary>
    public AChannelId()
    {
        _channelCount = 0;
        _index = 0;
    }

    /// <summary>
    /// 
    /// Use <see cref="AChannelId.MonoTrack"/> instead of this. Return channel index structure when it's an only channel inside mono audio frame.
    /// 
    /// </summary>
    /// <param name="index">Must be 0.</param>
    private AChannelId(int index)
    {
        Debug.Assert(index == 0);
        _channelCount = 1;
        _index = 0;
    }

    public AChannelId(int index, AChannelLayout format)
    {
        Debug.Assert(index >= 0 && index < format.Count);
        _channelCount = format.Count;
        _index = index;
    }

    public AChannelId(int index, int count)
    {
        Debug.Assert(index >= 0 && index < count);
        _channelCount = count;
        _index = index;
    }
}
