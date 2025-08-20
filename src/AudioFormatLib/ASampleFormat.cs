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
    public static ASampleFormat DefaultForType<T>(this ASampleFormat id)
        where T : unmanaged
    {
        if (GenericType<T>.IsByte)
        {
            return ASampleFormat.U8;
        }
        if (GenericType<T>.IsDouble)
        {
            return ASampleFormat.DOUBLE;
        }
        if (GenericType<T>.IsFloat)
        {
            return ASampleFormat.FLOAT;
        }
        if (GenericType<T>.IsInt)
        {
            return ASampleFormat.S32;
        }
        if (GenericType<T>.IsShort)
        {
            return ASampleFormat.S16;
        }

        return ASampleFormat.NONE;
    }

    public static bool IsCompatible<T>(this ASampleFormat id)
        where T : unmanaged
    {
        switch (id)
        {
            case ASampleFormat.NONE:
                return false;

            case ASampleFormat.U8:
            case ASampleFormat.P_U8:
                return GenericType<T>.IsUInt8;

            case ASampleFormat.S16:
            case ASampleFormat.P_S16:
                return GenericType<T>.IsShort;

            case ASampleFormat.FLOAT:
            case ASampleFormat.P_FLOAT:
                return GenericType<T>.IsFloat;

            case ASampleFormat.S32:
            case ASampleFormat.P_S32:
                return GenericType<T>.IsInt;

            case ASampleFormat.DOUBLE:
            case ASampleFormat.P_DOUBLE:
                return GenericType<T>.IsDouble;

            case ASampleFormat.S64:
            case ASampleFormat.P_S64:
                return GenericType<T>.IsLong;

            default:
                throw new ArgumentException($"Invalid format identifier: {id}");
        }
    }

    public static int Bits(this ASampleFormat id)
    {
        switch (id)
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

    public static long ToByteCount(this ASampleFormat id, long sampleCount)
    {
        return id.Bits() * sampleCount / 8;
    }

    public static int ConverterIndex(this ASampleFormat id)
    {
        switch (id)
        {
            case ASampleFormat.FLOAT:
            case ASampleFormat.P_FLOAT:
                return 0;

            case ASampleFormat.S16:
            case ASampleFormat.P_S16:
                return 1;

            case ASampleFormat.NONE:
            case ASampleFormat.U8:
            case ASampleFormat.P_U8:
            case ASampleFormat.S32:
            case ASampleFormat.P_S32:
            case ASampleFormat.DOUBLE:
            case ASampleFormat.P_DOUBLE:
            case ASampleFormat.S64:
            case ASampleFormat.P_S64:
                return -1; // WIP

            default:
                throw new ArgumentException($"Invalid format identifier: {id}");
        }
    }
}
