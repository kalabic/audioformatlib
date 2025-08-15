using AudioFormatLib.Utils;

namespace AudioFormatLib.Converters;


/// <summary>
/// 
/// Convert samples from interleaved source channel into destination frame with a single channel.
/// 
/// </summary>
public static class InterleavedToPlanar
{
    public static unsafe void Float_To_ShortPtr(ConverterParams context, float[] input, long offset, long length, short* output, long outOffset)
    {
        fixed (float* inputPtr = input)
        {
            FloatPtr_To_ShortPtr_WithOffset(context.SrcChannel.ChannelCount, inputPtr + offset * context.SrcChannel.ChannelCount + context.SrcChannel.Index, output + outOffset, (int)length);
        }
    }

    public static unsafe void FloatPtr_To_ShortPtr_WithOffset(int cn, float* input, short* output, int length)
    {
        for (int i = 0; i < length; i++)
        {
            *output++ = (short)(*input * ConverterParams.CONVERT_FACTOR_SHORT);
            input += cn;
        }
    }

    public static unsafe void ShortPtr_To_Float(ConverterParams context, short* input, long offset, long length, float[] output, long outOffset)
    {
        fixed (float* outputPtr = output)
        {
            ShortPtr_To_FloatPtr_WithOffset(
                context.SrcChannel.ChannelCount, input + offset * context.SrcChannel.ChannelCount + context.SrcChannel.Index, outputPtr + outOffset, (int)length);
        }
    }

    public static unsafe void ShortPtr_To_FloatPtr_WithOffset(int cn, short* input, float* output, int length)
    {
        for (int i = 0; i < length; i++)
        {
            *output++ = *input / ConverterParams.CONVERT_FACTOR_SHORT;
            input += cn;
        }
    }
}
