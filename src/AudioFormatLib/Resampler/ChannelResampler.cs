/******************************************************************************
 *
 * AudioFormatLib
 * Copyright (c) 2025 Silvije Kalabic
 *
 * libresamplesharp
 * Copyright (c) 2018 Xianyi Cui
 *
 * libresample4j
 * Copyright (c) 2009 Laszlo Systems, Inc
 *
 * libresample 0.1.3
 * Copyright (c) 2003 Dominic Mazzoni
 *
 * Resample 1.7
 * Copyright (c) 1994-2002 Julius O. Smith III
 * 
 * 'AudioFormatLib' is using C# code that is a fork from 'libresamplesharp'.
 * 'libresamplesharp' is a C# port of David Nault's 'libresample4j',
 * which is a Java port of Dominic Mazzoni's 'libresample 0.1.3',
 * which is in turn based on Julius Smith's 'Resample 1.7' library.
 *
 * This product includes software derived from the work of
 * Julius Smith, Dominic Mazzoni, David Nault, and others.
 * 
 * - https://ccrma.stanford.edu/~jos/resample/
 * - https://ccrma.stanford.edu/~jos/resample/Free_Resampling_Software.html
 * - https://github.com/minorninth/libresample
 * - https://github.com/dnault/libresample4j
 * - https://github.com/xycui/libresamplesharp
 *
 *
 * This file is part of AudioFormatLib, which is free software:
 * you can redistribute it and/or modify it under the terms of the
 * GNU Lesser General Public License (LGPL), version 2.1 only,
 * as published by the Free Software Foundation.
 *
 * AudioFormatLib is intended to be used as a dynamically linked library.
 * Applications using this library are not subject to the LGPL license,
 * provided they comply with its terms (e.g., allowing replacement of the library).
 *
 * AudioFormatLib is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * version 2.1 along with this file. If not, see:
 * https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html 
 *
 *****************************************************************************/

using AudioFormatLib.IO;


namespace AudioFormatLib.Resampler
{
    /// <summary>
    /// 
    /// Single audio channel resampler is a wrapper around instance of <see cref="ReSampler"/> and
    /// provides it with an interface for on-the-fly conversion of input and output sample data.
    /// 
    /// <para> For now, the only supported sample format is signed 16-bit. </para>
    /// 
    /// </summary>
    public unsafe class ChannelResampler
    {
        public long SamplesRead { get { return _samplesRead; } }

        public long SamplesWritten { get { return _samplesWritten; } }


        private ReSampler _resampler;

        private long _samplesRead = 0;

        private long _samplesWritten = 0;


        /// <summary>
        /// 
        /// Create indenpendent instance of <see cref="ReSampler"/>. It is necessary to create an instance for
        /// each and every channel in case of multi-channel audio frame.
        /// 
        /// </summary>
        /// <param name="highQuality"></param>
        /// <param name="factor"></param>
        internal ChannelResampler(bool highQuality, float factor)
        {
            _resampler = new(highQuality, factor, factor);
        }


        internal unsafe bool ProcessInput(float factor, bool lastPacket, IInputProducer<float> producer, IOutputConsumer<float> consumer)
        {
            bool result = _resampler.Process(factor, producer, consumer, lastPacket);
            _samplesRead += producer.SamplesRead;
            _samplesWritten += consumer.SamplesWritten;
            return result;
        }
    }
}
