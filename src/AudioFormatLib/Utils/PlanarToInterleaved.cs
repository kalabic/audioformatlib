namespace AudioFormatLib.Utils;


/// <summary>
/// 
/// Convert samples from a simple single-channel source frame into destination frame with
/// a number of interleaved channels.
/// 
/// </summary>
public static class PlanarToInterleaved
{
    public static unsafe void Float_To_ShortPtr(ConverterParams context, float[] input, long offset, long length, short* output, long outOffset)
    {
        fixed (float* inputPtr = input)
        {
            FloatPtr_To_ShortPtr_WithOffset(
                context.DstChannel.ChannelCount, inputPtr + offset, output + (outOffset * context.DstChannel.ChannelCount) + context.DstChannel.Index, (int)length);
        }
    }

    public static unsafe void FloatPtr_To_ShortPtr_WithOffset(int cn, float* input, short* output, int length)
    {
        for (int i = 0; i < length; i++)
        {
            *output = (short)( (*input) * ConverterParams.CONVERT_FACTOR_SHORT );
            input++;
            output += cn;
        }
    }

    public static unsafe void ShortPtr_To_Float(ConverterParams context, short* input, long offset, long length, float[] output, long outOffset)
    {
        fixed (float* outputPtr = output)
        {
            ShortPtr_To_FloatPtr_WithOffset(
                context.DstChannel.ChannelCount, input + offset, outputPtr + (outOffset * context.DstChannel.ChannelCount) + context.DstChannel.Index, (int)length);
        }
    }

    public static unsafe void ShortPtr_To_FloatPtr_WithOffset(int cn, short* input, float* output, int length)
    {
        for (int i = 0; i < length; i++)
        {
            *output = (*input++) / ConverterParams.CONVERT_FACTOR_SHORT;
            output += cn;
        }
    }
}
