using AudioFormatLib.Buffers;
using AudioFormatLib.Extensions;
using AudioFormatLib.IO;
using AudioFormatLib.Utils;
using System.Diagnostics;

namespace AudioFormatLib;


public static class ATools
{
    /// <summary> WIP </summary>
    public static void Convert(in AudioSpan source, in AudioSpan destination)
    {
        unsafe
        {
            var CP = ChannelConverter.Get_Func(source, destination);
            CP.Func(CP, source, destination);
        }
    }

    public static void ConvertToStereo(in AudioSpan leftChannel, in AudioSpan rightChannel, in AudioSpan destination)
    {
        Convert(leftChannel, destination, AChannelId.LeftStereo);
        Convert(rightChannel, destination, AChannelId.RightStereo);
    }

    public static void Convert(in AudioSpan source, in AudioSpan destination,
                               in AChannelId sourceChannel, in AChannelId destinationChannel)
    {
        unsafe
        {
            var CP = ChannelConverter.Get_Func(source, destination, sourceChannel, destinationChannel);
            CP.Func(CP, source, destination);
        }
    }

    public static void Convert(in AudioSpan source, in AudioSpan destination, in AChannelId destinationChannel)
    {
        Convert(source, destination, AChannelId.MonoTrack, destinationChannel);
    }


    /// <summary> WIP </summary>
    public static unsafe void Convert(byte* input, long offset, long length, AFrameFormat inputFormat,
                                      byte* output, long outOffset, long outLength, AFrameFormat outputFormat)
    {
        var inSpan = new AudioSpan(input, offset, length, inputFormat);
        var outSpan = new AudioSpan(output, outOffset, outLength, outputFormat);
        Convert(inSpan, outSpan);
    }


    /// <summary> WIP </summary>
    public static void Convert(in AudioPtr input, long offset, long length,
                               in AudioPtr output, long outOffset, long outLength)
    {
        var inSpan = new AudioSpan(input, offset, length);
        var outSpan = new AudioSpan(output, outOffset, outLength);
        Convert(inSpan, outSpan);
    }


    /// <summary> WIP </summary>
    public static void Convert<IN, OUT>(in AudioPtr.Any<IN> input, long offset, long length, AFrameFormat inputFormat,
                                        in AudioPtr.Any<OUT> output, long outOffset, long outLength, AFrameFormat outputFormat)
         where IN : unmanaged
         where OUT : unmanaged
    {
        using var fixedInput = input.MakeFixed();
        using var fixedOutput = output.MakeFixed();
        unsafe
        {
            var inputSpan = new AudioSpan((byte*)fixedInput, offset * sizeof(IN), length * sizeof(IN), inputFormat);
            var outputSpan = new AudioSpan((byte*)fixedOutput, outOffset * sizeof(OUT), outLength * sizeof(OUT), outputFormat);
            Convert(inputSpan, outputSpan);
        }
    }


    /// <summary> WIP </summary>
    public static void Convert<IN, OUT>(ReadOnlySpan<IN> input, AFrameFormat inputFormat,
                                        Span<OUT> output, AFrameFormat outputFormat)
        where IN : unmanaged
        where OUT : unmanaged
    {
        unsafe
        {
            fixed (IN* inPtr = input)
            fixed (OUT* outPtr = output)
            {
                var inSpan = new AudioSpan((byte*)inPtr, 0, input.Length * sizeof(IN), inputFormat);
                var outSpan = new AudioSpan((byte*)outPtr, 0, output.Length * sizeof(OUT), outputFormat);
                Convert(inSpan, outSpan);
            }
        }
    }


    public static ChannelMapping ValidateChannelMapping(AChannelId source, AChannelId destination)
    {
        if (source.AllChannels && destination.AllChannels)
        {
            return ChannelMapping.PLANAR;
        }
        Debug.Assert(!source.AllChannels && !destination.AllChannels, "Incompatible source and destination for conversion.");
        if (source.AllChannels || destination.AllChannels)
        {
            return ChannelMapping.UNSUPPORTED;
        }

        Debug.Assert(source.ChannelCount >= 1 && destination.ChannelCount >= 1);
        Debug.Assert(source.Index >= 0 && source.Index < source.ChannelCount);
        Debug.Assert(destination.Index >= 0 && destination.Index < destination.ChannelCount);

        if (source.Index < 0 || source.Index >= source.ChannelCount)
        {
            return ChannelMapping.UNSUPPORTED;
        }
        if (destination.Index < 0 || destination.Index >= destination.ChannelCount)
        {
            return ChannelMapping.UNSUPPORTED;
        }
        if (source.ChannelCount == 1 && destination.ChannelCount == 1)
        {
            return ChannelMapping.PLANAR;
        }
        if (source.ChannelCount == 1 && destination.ChannelCount > 1)
        {
            return ChannelMapping.PLANAR_TO_INTERLEAVED;
        }
        if (source.ChannelCount > 1 && destination.ChannelCount == 1)
        {
            return ChannelMapping.INTERLEAVED_TO_PLANAR;
        }
        if (source.ChannelCount > 1 && destination.ChannelCount > 1)
        {
            return ChannelMapping.INTERLEAVED;
        }

        return ChannelMapping.UNSUPPORTED;
    }


    /// <summary>
    /// 
    /// As per function name, calculate expected output size for given input size and resampling factor.
    /// 
    /// </summary>
    /// <param name="inputSize"></param>
    /// <param name="factor"></param>
    /// <param name="frameSize">Sample size in bytes multiplied with number of channels.</param>
    /// <returns></returns>
    public static long GetExpectedByteSize(long inputSize, float factor, int frameSize)
    {
        if (inputSize <= 0 || factor <= 0.0f || frameSize <= 0)
        {
            Debug.Fail("Parameteres for GetExpectedByteSize() function cannot be negative or 0.");
            return 0;
        }

        Debug.Assert(inputSize % frameSize == 0, $"Provided input size does not contain a complete frame.");
        long inputFrames = inputSize / frameSize;
        long outputFrames = (long)((double)inputFrames * factor) + 200;
        return outputFrames * frameSize;
    }


    /// <summary>
    /// 
    /// This is used for validation of some of parameters used to create a resampler. Values that need to be provided are:
    /// 
    /// <list type="bullet">
    ///     <item>only <paramref name="factor"/></item>
    ///     <item>only <paramref name="inputSampleRate"/> and <paramref name="outputSampleRate"/></item>
    ///     <item>or <paramref name="factor"/> and only one of sample rates</item>
    /// </list>
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="inputSampleRate"></param>
    /// <param name="outputSampleRate"></param>
    /// <returns>Calculated and validated value for <paramref name="factor"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static float CalcFactor(float factor = 0.0f, int inputSampleRate = 0, int outputSampleRate = 0)
    {
        if (factor > 0.0f && inputSampleRate > 0 && outputSampleRate > 0)
        {
            throw new ArgumentException("Provide only factor, only input and output sample rates, or only factor and one of sample rates.");
        }

        if (factor <= 0.0f)
        {
            if (inputSampleRate == 0 || outputSampleRate == 0)
            {
                throw new ArgumentException("If factor is unspecified, both input and output sample rates must be provided.");
            }

            factor = outputSampleRate / (float)inputSampleRate;
        }

        return factor;
    }


    /// <summary>
    /// 
    /// This is used for validation of some of parameters used to create a resampler. Values that need to be provided are:
    /// 
    /// <list type="bullet">
    ///     <item>only <paramref name="factor"/></item>
    ///     <item>only <paramref name="inputSampleRate"/> and <paramref name="outputSampleRate"/></item>
    ///     <item>or <paramref name="factor"/> and only one of sample rates</item>
    /// </list>
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="inputSampleRate"></param>
    /// <param name="outputSampleRate"></param>
    /// <returns>Calculated and validated value for <paramref name="inputSampleRate"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static int CalcInputSampleRate(float factor, int inputSampleRate, int outputSampleRate)
    {
        if (factor > 0.0f && inputSampleRate > 0 && outputSampleRate > 0)
        {
            throw new ArgumentException("Provide only factor, only input and output sample rates, or only factor and one of sample rates.");
        }

        if (inputSampleRate <= 0)
        {
            if (factor <= 0.0f || outputSampleRate <= 0)
            {
                throw new ArgumentException("If input sample rate is unspecified, factor or both factor and output sample rate must be provided.");
            }

            inputSampleRate = (int)(outputSampleRate / factor);
        }

        return inputSampleRate;
    }


    /// <summary>
    /// 
    /// This is used for validation of some of parameters used to create a resampler. Values that need to be provided are:
    /// 
    /// <list type="bullet">
    ///     <item>only <paramref name="factor"/></item>
    ///     <item>only <paramref name="inputSampleRate"/> and <paramref name="outputSampleRate"/></item>
    ///     <item>or <paramref name="factor"/> and only one of sample rates</item>
    /// </list>
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="inputSampleRate"></param>
    /// <param name="outputSampleRate"></param>
    /// <returns>Calculated and validated value for <paramref name="outputSampleRate"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static int CalcOutputSampleRate(float factor, int inputSampleRate, int outputSampleRate)
    {
        if (factor > 0.0f && inputSampleRate > 0 && outputSampleRate > 0)
        {
            throw new ArgumentException("Provide only factor, only input and output sample rates, or only factor and one of sample rates.");
        }

        if (outputSampleRate <= 0)
        {
            if (factor <= 0.0f || inputSampleRate <= 0)
            {
                throw new ArgumentException("If output sample rate is unspecified, factor or both factor and input sample rate must be provided.");
            }

            outputSampleRate = (int)(inputSampleRate * factor);
        }

        return outputSampleRate;
    }

    internal static IUnsafeBuffer CreateUnsafeBuffer(ABufferParams bparams)
    {
        if (bparams.WaitForCompleteRead)
        {
            return new CircularBufferWaitable(bparams.BufferSize);
        }
        else
        {
            return new CircularBufferLocked(bparams.BufferSize);
        }
    }

    internal static AudioInputs CreateInputsWithBuffer(ABufferParams bparams, IUnsafeBuffer buffer)
    {
        return new AudioInputs(bparams, buffer);
    }

    internal static AudioOutputs CreateOutputsWithBuffer(ABufferParams bparams, IUnsafeBuffer buffer)
    {
        return new AudioOutputs(bparams, buffer);
    }

    internal static SampleProducer CreateSampleProducer(AChannelId channel)
    {
        var convertIn = ChannelConverter.Get_Func<short, float>(channel, AChannelId.MonoTrack);
        if (!convertIn.Valid)
        {
            throw new NotImplementedException("Sample conversion function not found.");
        }

        return new SampleProducer(convertIn);
    }

    internal static SampleConsumer CreateSampleConsumer(AChannelId channel)
    {
        var convertOut = ChannelConverter.Get_Func<float, short>(AChannelId.MonoTrack, channel);
        if (!convertOut.Valid)
        {
            throw new NotImplementedException("Sample conversion function not found.");
        }

        return new SampleConsumer(convertOut);
    }
}
