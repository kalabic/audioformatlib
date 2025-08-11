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
/// This structure holds a description of a channel layout inside audio frame.
/// 
/// </summary>
public struct AChannelLayout
{
    /// <summary> For convenience when dealing with a mono audio frame. </summary>
    public static readonly AChannelLayout MonoAudio = new AChannelLayout(1);

    /// <summary> For convenience when dealing with a stereo audio frame. </summary>
    public static readonly AChannelLayout StereoAudio = new AChannelLayout(2);



    /// <summary> For convenience when dealing with a mono audio frame. </summary>
    public AChannelId MonoTrack { get { Debug.Assert(_count == 1); return GetChannelIndex(0); } }

    /// <summary> For convenience when dealing with a stereo audio frame. </summary>
    public AChannelId Left { get { Debug.Assert(_count == 2); return GetChannelIndex(0); } }

    /// <summary> For convenience when dealing with a stereo audio frame. </summary>
    public AChannelId Right { get { Debug.Assert(_count == 2); return GetChannelIndex(1); } }

    /// <summary> Number of channels in an audio frame. </summary>
    public int Count { get { return _count; } }


    private readonly int _count;

    public AChannelLayout(int count)
    {
        _count = count;
    }

    public AChannelId GetChannelIndex(int index)
    {
        return new AChannelId(index, this);
    }
}
