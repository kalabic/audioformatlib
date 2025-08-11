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

using AudioFormatLib.Utils;

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
        : ReSampler.IInputProducer
        , ReSampler.IOutputConsumer
        , IDisposable
    {
        private ReSampler _resampler;

        private ConverterParams _inputParams;

        private ConverterParams _outputParams;

        private AConversion_ShortPtr_To_Float _inputFunction;

        private AConversion_Float_To_ShortPtr _outputFunction;

        private short* _input = null;

        private short* _output = null;

        private long _inputSampleCount = 0;

        private long _inputSamplesUsed = 0;

        private long _outputSampleMaxCount = 0;

        private long _outputSampleCount = 0;


        /// <summary>
        /// 
        /// Create indenpendent instance of <see cref="ReSampler"/>. It is necessary to create an instance for
        /// each and every channel in case of multi-channel audio frame.
        /// 
        /// </summary>
        /// <param name="highQuality"></param>
        /// <param name="factor"></param>
        /// <param name="channel"> The same channel will be read from at input and written to at output. </param>
        /// <exception cref="NotImplementedException"></exception>
        internal ChannelResampler(bool highQuality, float factor, AChannelId channel)
        {
            _resampler = new(highQuality, factor, factor);

            var convertIn = ChannelConverter.Get_ShortPtr_To_Float_Func(channel, AChannelId.MonoTrack);
            var convertOut = ChannelConverter.Get_Float_To_ShortPtr_Func(AChannelId.MonoTrack, channel);
            if (convertIn is null || convertOut is null)
            {
                throw new NotImplementedException("Sample conversion function not found.");
            }

            _inputFunction = convertIn.Value.Func;
            _inputParams = convertIn.Value.Params;
            _outputFunction = convertOut.Value.Func;
            _outputParams = convertOut.Value.Params;
        }


        /// <summary>
        /// 
        /// Both <paramref name="input"/> and <paramref name="output"/> can be a multi-channel audio frame or
        /// a simple mono audio type. This function will access only the channel specified in constructor using
        /// provided channel id.
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="lastPacket"></param>
        /// <param name="input"></param>
        /// <param name="inputSize"></param>
        /// <param name="output"></param>
        /// <param name="outputSize"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal unsafe long ProcessInput(float factor, bool lastPacket, byte* input, long inputSize, byte* output, long outputSize)
        {
            _input = (short *)input;
            _output = (short *)output;
            _inputSamplesUsed = 0;
            _inputSampleCount = inputSize / sizeof(short);
            _outputSampleCount = 0;
            _outputSampleMaxCount = outputSize / sizeof(short);

            _resampler.Process(factor, this, this, lastPacket);
            if (_inputSamplesUsed < _inputSampleCount)
            {
                throw new InvalidOperationException("Previous operation did not process all of input.");
            }

            return _outputSampleCount * sizeof(short);
        }

        /// <summary> See summary for <see cref="ReSampler.IInputProducer.GetInputBufferLength"/> </summary>
        public int GetInputBufferLength()
        {
            return (int)(_inputSampleCount - _inputSamplesUsed);
        }

        /// <summary> See summary for <see cref="ReSampler.IOutputConsumer.GetOutputBufferLength"/> </summary>
        public int GetOutputBufferLength()
        {
            return (int)(_outputSampleMaxCount - _outputSampleCount);
        }

        /// <summary> See summary for <see cref="ReSampler.IInputProducer.ProduceInput"/> </summary>
        public void ProduceInput(float[] array, int offset, int length)
        {
            _inputFunction(_inputParams, _input, _inputSamplesUsed, length, array, offset);
            _inputSamplesUsed += length;
        }

        /// <summary> See summary for <see cref="ReSampler.IOutputConsumer.ConsumeOutput"/> </summary>
        public void ConsumeOutput(float[] array, int offset, int length)
        {
            _outputFunction(_outputParams, array, offset, length, _output, _outputSampleCount);
            _outputSampleCount += length;
        }

        public void Dispose()
        {
        }
    }
}
