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

using AudioFormatLib.Utils;

namespace AudioFormatLib;


/// <summary>
/// 
/// Represents a method that converts a segment of audio samples from a managed 
/// float array to an unmanaged pointer to 16-bit signed integer (short) samples.
/// This delegate is typically used for converting floating-point PCM audio data 
/// (commonly in range [-1.0, 1.0]) into 16-bit PCM format suitable for output or
/// interop with native APIs.
/// 
/// </summary>
/// <param name="cp">Context or configuration parameters required for the conversion process.</param>
/// <param name="input">The source buffer containing floating-point audio samples.</param>
/// <param name="offset">The starting index within the input array from which conversion begins.</param>
/// <param name="length">The number of samples to convert from the input buffer.</param>
/// <param name="output">Pointer to the destination buffer where converted 16-bit samples will be written.</param>
/// <param name="outOffset">Offset in the destination buffer where writing should begin.</param>
/// 
public unsafe delegate void AConversion_Float_To_ShortPtr(
    ConverterParams cp,
    float[] input,
    long offset,
    long length,
    byte* output,
    long outOffset
);


/// <summary>
/// 
/// Represents a method that converts a segment of audio samples from an unmanaged
/// pointer to 16-bit signed integer (short) samples into a managed float array.
/// This delegate is commonly used for converting raw PCM 16-bit audio data into
/// floating-point format, which is useful for processing or analysis in managed
/// code.
/// 
/// </summary>
/// <param name="cp">Context or configuration parameters required for the conversion process.</param>
/// <param name="input">Pointer to the source buffer containing 16-bit PCM audio samples.</param>
/// <param name="offset">The starting index within the input buffer from which conversion begins.</param>
/// <param name="length">The number of samples to convert from the input buffer.</param>
/// <param name="output">The destination buffer where converted floating-point samples will be stored.</param>
/// <param name="outOffset">Offset in the destination array where writing should begin.</param>
/// 
public unsafe delegate void AConversion_ShortPtr_To_Float(
    ConverterParams cp,
    byte* input,
    long offset,
    long length,
    float[] output,
    long outOffset
);
