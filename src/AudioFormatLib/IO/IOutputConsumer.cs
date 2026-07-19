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
/// Callback for consuming scalar sample values. Enables on-the-fly conversion between value types
/// (signed 16-bit integers to floats, for example) and/or writing directly to an output stream.
/// </summary>
public interface IOutputConsumer<T> where T : unmanaged
{
    long SampleValuesWritten { get; }

    /// <summary>
    /// Get the number of sample values the output buffer has room for.
    /// </summary>
    /// <returns>Number of sample values the output buffer has room for.</returns>
    long GetOutputBufferLength();

    /// <summary>
    /// Copy sample values from the given array to the output buffer.
    /// </summary>
    /// <param name="array">Array containing sample values.</param>
    /// <param name="sampleValueOffset">Source offset measured in sample values.</param>
    /// <param name="sampleValueCount">Number of sample values to consume.</param>
    void ConsumeOutput(T[] array, int sampleValueOffset, int sampleValueCount);
}
