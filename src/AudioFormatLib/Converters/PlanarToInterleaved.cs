using AudioFormatLib.Utils;

namespace AudioFormatLib.Converters;


/// <summary>
/// 
/// Convert samples from a simple single-channel source frame into destination frame with
/// a number of interleaved channels.
/// 
/// </summary>
public static class PlanarToInterleaved
{
    public static unsafe void Float_To_ShortPtr(ConverterParams context, float[] input, long offset, long length, byte* output, long outOffset)
    {
        fixed (float* inputPtr = input)
        {
            int ccOut = context.DstChannel.ChannelCount;
            float* offsetInputPtr = inputPtr + offset;
            short* offsetOutputPtr = (short*)output + outOffset * ccOut + context.DstChannel.Index;
            FloatPtr_To_ShortPtr_WithOffset(
                ccOut, (byte*)offsetInputPtr, (byte*)offsetOutputPtr, (int)length);
        }
    }

    public static unsafe void FloatPtr_To_ShortPtr_WithOffset(int ccOut, byte* input, byte* output, int length)
    {
        float* floatIn = (float*)input;
        short* shortOut = (short*)output;
        for (int i = 0; i < length; i++)
        {
            *shortOut = (short)(*floatIn++ * ConverterParams.CONVERT_FACTOR_SHORT );
            shortOut += ccOut;
        }
    }

    public static unsafe void ShortPtr_To_Float(ConverterParams context, byte* input, long offset, long length, float[] output, long outOffset)
    {
        fixed (float* outputPtr = output)
        {
            int ccOut = context.DstChannel.ChannelCount;
            short* offsetInputPtr = (short*)input + offset;
            float* offsetOutputPtr = outputPtr + outOffset * ccOut + context.DstChannel.Index;
            ShortPtr_To_FloatPtr_WithOffset(
                ccOut, (byte*)offsetInputPtr, (byte*)offsetOutputPtr, (int)length);
        }
    }

    public static unsafe void ShortPtr_To_FloatPtr_WithOffset(int ccOut, byte* input, byte* output, int length)
    {
        short* shortIn = (short*)input;
        float* floatOut = (float*)output;
        for (int i = 0; i < length; i++)
        {
            *floatOut = *shortIn++ / ConverterParams.CONVERT_FACTOR_SHORT;
            floatOut += ccOut;
        }
    }
}
