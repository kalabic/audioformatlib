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
/// Callback for consuming samples. Enalbes on-the-fly conversion between sample types
/// (signed 16-bit integers to floats, for example) and/or writing directly to an output stream.
/// </summary>
internal interface IOutputConsumer<T> where T : unmanaged
{
    long SamplesWritten { get; }

    /// <summary>
    /// Get the number of samples the output buffer has room for
    /// </summary>
    /// <returns>number of samples the output buffer has room for</returns>
    long GetOutputBufferLength();

    /// <summary>
    /// Copy length samples from the given array to the output buffer, starting at the given offset.
    /// </summary>
    /// <param name="array">array to read from</param>
    /// <param name="offset">start reading samples here</param>
    /// <param name="length">read this many samples</param>
    void ConsumeOutput(T[] array, int offset, int length);
}
