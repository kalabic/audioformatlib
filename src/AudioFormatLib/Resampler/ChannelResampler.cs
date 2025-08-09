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

namespace AudioFormatLib.Resampler
{
    /// <summary>
    /// Every audio channel has its own indenpendent resampler. It is a wrapper around instance of <see cref="ReSampler"/> and
    /// provides it with an interface for on-the-fly conversion.
    /// </summary>
    public unsafe class ChannelResampler 
        : ReSampler.IInputProducer
        , ReSampler.IOutputConsumer
        , IDisposable
    {
        /// <summary>
        /// Value used by Apple (Core Audio)1, ALSA2, MatLab2, sndlib2.
        /// <para>[1] Link: <a href="https://web.archive.org/web/20210210064026/http://blog.bjornroche.com/2009/12/int-float-int-its-jungle-out-there.html">Int->Float->Int: It's a jungle out there! (Web Archive)</a></para>
        /// <para>[2] Link: <a href="http://www.mega-nerd.com/libsndfile/FAQ.html#Q010">Q10: Reading a 16 bit PCM file as normalised floats...</a></para>
        /// </summary>
        private const float CONVERT_FACTOR_SHORT = 32768.0f;


        private ReSampler _resampler;

        private int _ioff;

        private int _istep;

        private short* _input = null;

        private short* _output = null;

        private long _inputSampleCount = 0;

        private long _inputSamplesUsed = 0;

        private long _outputSampleMaxCount = 0;

        private long _outputSampleCount = 0;

        internal ChannelResampler(bool highQuality, float factor, int ioff, int istep)
        {
            _resampler = new(highQuality, factor, factor);
            _ioff = ioff;
            _istep = istep;
        }

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

        /// <summary> See summary for <see cref="ReSampler.IInputProducer.GetInputBufferLenght"/> </summary>
        public int GetInputBufferLenght()
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
            for (int i = 0; i < length; i++)
            {
                int index = (int)(_ioff + ((_inputSamplesUsed + i) * _istep));
                array[offset + i] = _input[index] / CONVERT_FACTOR_SHORT;
            }
            _inputSamplesUsed += length;
        }

        /// <summary> See summary for <see cref="ReSampler.IOutputConsumer.ConsumeOutput"/> </summary>
        public void ConsumeOutput(float[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                int index = (int)(_ioff + ((_outputSampleCount + i) * _istep));
                _output[index] = (short)(array[offset + i] * CONVERT_FACTOR_SHORT);
            }
            _outputSampleCount += length;
        }

        public void Dispose()
        {
        }
    }
}
