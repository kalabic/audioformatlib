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

namespace AudioFormatLib;


/// <summary>
/// 
/// Work in progress. Intention is to have similar features here like equivalent
/// structures found in FFmpeg and other libraries.
/// 
/// </summary>
public struct AChannelFormat
{
    public int Count { get { return _count; } }

    private readonly int _count;

    public AChannelFormat(int count)
    {
        _count = count;
    }
}
