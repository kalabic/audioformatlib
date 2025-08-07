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
/// A sample represents single scalar amplitude value at a specific timestamp. Intention is
/// to keep these values the same/compatible as in AVSampleFormat found in FFmpeg library.
/// </summary>
public enum ASampleFormat : int
{
    NONE = -1,

    /// <summary> Unsigned 8 bits. </summary>
    U8 = 0,

    /// <summary> Signed 16 bits. </summary>
    S16 = 1,

    /// <summary> Signed 32 bits. </summary>
    S32 = 2,

    /// <summary> Float. </summary>
    FLOAT = 3,

    /// <summary> Double. </summary>
    DOUBLE = 4,

    /// <summary> Unsigned 8 bits, planar. </summary>
    P_U8 = 5,

    /// <summary> Signed 16 bits, planar. </summary>
    P_S16 = 6,

    /// <summary> Signed 32 bits, planar. </summary>
    P_S32 = 7,

    /// <summary> Float, planar. </summary>
    P_FLOAT = 8,

    /// <summary> Double, planar. </summary>
    P_DOUBLE = 9,

    /// <summary> Signed 64 bits. </summary>
    S64 = 10,

    /// <summary> Signed 64 bits, planar. </summary>
    P_S64 = 11,
}

public static class ASampleFormatMethods
{
    public static int Bits(this ASampleFormat id)
    {
        switch(id)
        {
            case ASampleFormat.NONE:
                return 0;

            case ASampleFormat.U8:
            case ASampleFormat.P_U8:
                return 8;

            case ASampleFormat.S16:
            case ASampleFormat.P_S16:
                return 16;

            case ASampleFormat.FLOAT:
            case ASampleFormat.P_FLOAT:
            case ASampleFormat.S32:
            case ASampleFormat.P_S32:
                return 32;

            case ASampleFormat.DOUBLE:
            case ASampleFormat.P_DOUBLE:
            case ASampleFormat.S64:
            case ASampleFormat.P_S64:
                return 64;

            default:
                throw new ArgumentException($"Invalid format identifier: {id}");
        }
    }

    public static int Size(this ASampleFormat id)
    {
        return id.Bits() / 8;
    }
}
