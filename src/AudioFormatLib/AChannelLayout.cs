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
/// Describes the channels and planar/interleaved storage of PCM samples.
/// 
/// </summary>
public struct AChannelLayout
{
    /// <summary> For convenience when dealing with mono PCM. </summary>
    public static readonly AChannelLayout MonoAudio = new AChannelLayout(1);

    /// <summary> For convenience when dealing with stereo PCM. </summary>
    public static readonly AChannelLayout StereoAudio = new AChannelLayout(2);



    /// <summary> For convenience when dealing with a mono sample. </summary>
    public AChannelId MonoTrack { get { Debug.Assert(_count == 1); return GetChannelIndex(0); } }

    /// <summary> For convenience when dealing with a stereo sample. </summary>
    public AChannelId Left { get { Debug.Assert(_count == 2); return GetChannelIndex(0); } }

    /// <summary> For convenience when dealing with a stereo sample. </summary>
    public AChannelId Right { get { Debug.Assert(_count == 2); return GetChannelIndex(1); } }

    /// <summary> Number of sample values in each sample. </summary>
    public int Count { get { return _count; } }

    /// <summary> WIP: Pretty ignorant (for now). </summary>
    public APcmLayout Layout { get { return _layout; } }

    /// <summary> Single-channel PCM is always considered planar. </summary>
    public bool IsPlanar { get { return _layout == APcmLayout.PLANAR; } }

    /// <summary> True when channel sample values are interleaved. </summary>
    public bool IsInterleaved { get { return _layout == APcmLayout.INTERLEAVED; } }


    private readonly int _count;

    private readonly APcmLayout _layout;

    /// <summary>
    /// 
    /// Layout will always be set to <see cref="APcmLayout.PLANAR"/> if
    /// <paramref name="count"/> is equal to 1.
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <param name="planar"></param>
    public AChannelLayout(int count, bool planar = false)
    {
        _count = count;
        _layout = (planar || (count == 1)) ? APcmLayout.PLANAR : APcmLayout.INTERLEAVED;
    }

    public AChannelId GetChannelIndex(int index)
    {
        return new AChannelId(index, this);
    }
}
