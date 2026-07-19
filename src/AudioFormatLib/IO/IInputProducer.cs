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
/// Callback for producing scalar sample values. Enables on-the-fly conversion between value types
/// (signed 16-bit integers to floats, for example).
/// </summary>
public interface IInputProducer<T> where T : unmanaged
{
    long SampleValuesRead { get; }

    /// <summary>
    /// Get the number of input sample values available.
    /// </summary>
    /// <returns>Number of input sample values available.</returns>
    long GetInputBufferLength();

    /// <summary>
    /// Copy sample values from the input buffer. Floating-point values are normalized to
    /// the range -1.0f to 1.0f.
    /// </summary>
    /// <param name="array">Array that receives sample values.</param>
    /// <param name="sampleValueOffset">Destination offset measured in sample values.</param>
    /// <param name="sampleValueCount">Number of sample values to produce.</param>
    void ProduceInput(T[] array, int sampleValueOffset, int sampleValueCount);
}
