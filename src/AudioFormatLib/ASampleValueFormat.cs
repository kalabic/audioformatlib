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

using AudioFormatLib.Extensions;

namespace AudioFormatLib;


/// <summary>
/// Describes the scalar representation of one channel's sample value. Values intentionally
/// remain compatible with AVSampleFormat in FFmpeg.
/// </summary>
public enum ASampleValueFormat : int
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

public static class ASampleValueFormatMethods
{
    public static ASampleValueFormat DefaultForType<T>(this ASampleValueFormat id)
        where T : unmanaged
    {
        if (GenericType<T>.IsByte)
        {
            return ASampleValueFormat.U8;
        }
        if (GenericType<T>.IsDouble)
        {
            return ASampleValueFormat.DOUBLE;
        }
        if (GenericType<T>.IsFloat)
        {
            return ASampleValueFormat.FLOAT;
        }
        if (GenericType<T>.IsInt)
        {
            return ASampleValueFormat.S32;
        }
        if (GenericType<T>.IsShort)
        {
            return ASampleValueFormat.S16;
        }

        return ASampleValueFormat.NONE;
    }

    public static bool IsCompatible<T>(this ASampleValueFormat id)
        where T : unmanaged
    {
        switch (id)
        {
            case ASampleValueFormat.NONE:
                return false;

            case ASampleValueFormat.U8:
            case ASampleValueFormat.P_U8:
                return GenericType<T>.IsUInt8;

            case ASampleValueFormat.S16:
            case ASampleValueFormat.P_S16:
                return GenericType<T>.IsShort;

            case ASampleValueFormat.FLOAT:
            case ASampleValueFormat.P_FLOAT:
                return GenericType<T>.IsFloat;

            case ASampleValueFormat.S32:
            case ASampleValueFormat.P_S32:
                return GenericType<T>.IsInt;

            case ASampleValueFormat.DOUBLE:
            case ASampleValueFormat.P_DOUBLE:
                return GenericType<T>.IsDouble;

            case ASampleValueFormat.S64:
            case ASampleValueFormat.P_S64:
                return GenericType<T>.IsLong;

            default:
                throw new ArgumentException($"Invalid format identifier: {id}");
        }
    }

    public static int Bits(this ASampleValueFormat id)
    {
        switch (id)
        {
            case ASampleValueFormat.NONE:
                return 0;

            case ASampleValueFormat.U8:
            case ASampleValueFormat.P_U8:
                return 8;

            case ASampleValueFormat.S16:
            case ASampleValueFormat.P_S16:
                return 16;

            case ASampleValueFormat.FLOAT:
            case ASampleValueFormat.P_FLOAT:
            case ASampleValueFormat.S32:
            case ASampleValueFormat.P_S32:
                return 32;

            case ASampleValueFormat.DOUBLE:
            case ASampleValueFormat.P_DOUBLE:
            case ASampleValueFormat.S64:
            case ASampleValueFormat.P_S64:
                return 64;

            default:
                throw new ArgumentException($"Invalid format identifier: {id}");
        }
    }

    public static int Size(this ASampleValueFormat id)
    {
        return id.Bits() / 8;
    }

    public static long ToByteCount(this ASampleValueFormat id, long sampleValueCount)
    {
        return id.Bits() * sampleValueCount / 8;
    }

    public static int ConverterIndex(this ASampleValueFormat id)
    {
        switch (id)
        {
            case ASampleValueFormat.FLOAT:
            case ASampleValueFormat.P_FLOAT:
                return 0;

            case ASampleValueFormat.S16:
            case ASampleValueFormat.P_S16:
                return 1;

            case ASampleValueFormat.NONE:
            case ASampleValueFormat.U8:
            case ASampleValueFormat.P_U8:
            case ASampleValueFormat.S32:
            case ASampleValueFormat.P_S32:
            case ASampleValueFormat.DOUBLE:
            case ASampleValueFormat.P_DOUBLE:
            case ASampleValueFormat.S64:
            case ASampleValueFormat.P_S64:
                return -1; // WIP

            default:
                throw new ArgumentException($"Invalid format identifier: {id}");
        }
    }
}
