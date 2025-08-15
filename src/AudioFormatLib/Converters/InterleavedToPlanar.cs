using AudioFormatLib.Utils;

namespace AudioFormatLib.Converters;


/// <summary>
/// 
/// Convert samples from interleaved source channel into destination frame with a single channel.
/// 
/// </summary>
public static class InterleavedToPlanar
{
    public static unsafe void Float_To_ShortPtr(ConverterParams context, float[] input, long offset, long length, byte* output, long outOffset)
    {
        fixed (float* inputPtr = input)
        {
            int ccIn = context.SrcChannel.ChannelCount;
            float* offsetInputPtr = inputPtr + offset * ccIn + context.SrcChannel.Index;
            short* offsetOutputPtr = (short*)output + outOffset;
            FloatPtr_To_ShortPtr_WithOffset(
                ccIn, (byte*)offsetInputPtr, (byte*)offsetOutputPtr, (int)length);
        }
    }

    public static unsafe void FloatPtr_To_ShortPtr_WithOffset(int ccIn, byte* input, byte* output, int length)
    {
        float* floatIn = (float*)input;
        short* shortOut = (short*)output;
        for (int i = 0; i < length; i++)
        {
            *shortOut++ = (short)(*floatIn * ConverterParams.CONVERT_FACTOR_SHORT);
            floatIn += ccIn;
        }
    }

    public static unsafe void ShortPtr_To_Float(ConverterParams context, byte* input, long offset, long length, float[] output, long outOffset)
    {
        fixed (float* outputPtr = output)
        {
            int ccIn = context.SrcChannel.ChannelCount;
            short* offsetIn = (short *)input + offset * ccIn + context.SrcChannel.Index;
            float* offsetOut = outputPtr + outOffset;
            ShortPtr_To_FloatPtr_WithOffset(
                ccIn, (byte *)offsetIn, (byte *)offsetOut, (int)length);
        }
    }

    public static unsafe void ShortPtr_To_FloatPtr_WithOffset(int ccIn, byte* input, byte* output, int length)
    {
        short* shortIn = (short *)input;
        float* floatOut = (float *)output;
        for (int i = 0; i < length; i++)
        {
            *floatOut++ = *shortIn / ConverterParams.CONVERT_FACTOR_SHORT;
            shortIn += ccIn;
        }
    }
}
