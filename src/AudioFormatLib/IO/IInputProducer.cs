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

namespace AudioFormatLib.IO;


/// <summary>
/// Callback for producing samples. Enalbes on-the-fly conversion between sample types
/// (signed 16-bit integers to floats, for example).
/// </summary>
internal interface IInputProducer<T> where T : unmanaged
{
    long SamplesRead { get; }

    /// <summary>
    /// Get the number of input samples available
    /// </summary>
    /// <returns>number of input samples available</returns>
    long GetInputBufferLength();

    /// <summary>
    /// Copy length samples from the input buffer to the given array, starting at the 
    /// given offset. Samples should be in the range -1.0f to 1.0f
    /// </summary>
    /// <param name="array">array to hold samples from the input buffer</param>
    /// <param name="offset">start writing samples here</param>
    /// <param name="length">write this many samples</param>
    void ProduceInput(T[] array, int offset, int length);
}
