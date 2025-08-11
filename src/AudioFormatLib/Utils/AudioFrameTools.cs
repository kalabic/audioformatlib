
using System.Diagnostics;

namespace AudioFormatLib.Utils;

public static class AudioFrameTools
{
    public static ChannelConverter.Mapping ValidateChannelMapping(AChannelId source, AChannelId destination)
    {
        Debug.Assert(source.ChannelCount >= 1 && destination.ChannelCount >= 1);
        Debug.Assert(source.Index >= 0 && source.Index < source.ChannelCount);
        Debug.Assert(destination.Index >= 0 && destination.Index < destination.ChannelCount);

        if (source.Index < 0 || source.Index >= source.ChannelCount)
        {
            return ChannelConverter.Mapping.UNSUPPORTED;
        }
        if (destination.Index < 0 || destination.Index >= destination.ChannelCount)
        {
            return ChannelConverter.Mapping.UNSUPPORTED;
        }
        if (source.ChannelCount == 1 && destination.ChannelCount == 1)
        {
            return ChannelConverter.Mapping.PLANAR;
        }
        if (source.ChannelCount == 1 && destination.ChannelCount > 1)
        {
            return ChannelConverter.Mapping.PLANAR_TO_INTERLEAVED;
        }
        if (source.ChannelCount > 1 && destination.ChannelCount == 1)
        {
            return ChannelConverter.Mapping.INTERLEAVED_TO_PLANAR;
        }
        if (source.ChannelCount > 1 && destination.ChannelCount > 1)
        {
            return ChannelConverter.Mapping.INTERLEAVED;
        }

        return ChannelConverter.Mapping.UNSUPPORTED;
    }


    /// <summary>
    /// 
    /// As per function name, calculate expected output size for given input size and resampling factor.
    /// 
    /// </summary>
    /// <param name="inputSize"></param>
    /// <param name="factor"></param>
    /// <returns></returns>
    public static long GetExpectedOutputSize(long inputSize, float factor)
    {
        return (long)((double)inputSize * factor) + 400;
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

            factor = (float)outputSampleRate / (float)inputSampleRate;
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

            inputSampleRate = (int)((float)outputSampleRate / factor);
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

            outputSampleRate = (int)((float)inputSampleRate * factor);
        }

        return outputSampleRate;
    }
}
