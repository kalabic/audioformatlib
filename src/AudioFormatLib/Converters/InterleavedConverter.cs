using AudioFormatLib.Utils;

namespace AudioFormatLib.Converters;


/// <summary>
/// 
/// Convert samples from interleaved input channel inside source frame into interleaved output
/// channel inside destination frame.
/// 
/// </summary>
public static class InterleavedConverter
{
    public static unsafe void Float_To_ShortPtr(ConverterParams context, float[] input, long offset, long length, short* output, long outOffset)
    {
        fixed (float* inputPtr = input)
        {
            FloatPtr_To_ShortPtr_WithOffset(
                context.SrcChannel.ChannelCount, context.DstChannel.ChannelCount,
                inputPtr + offset * context.SrcChannel.ChannelCount + context.SrcChannel.Index,
                output + outOffset * context.DstChannel.ChannelCount + context.DstChannel.Index,
                (int)length);
        }
    }

    public static unsafe void FloatPtr_To_ShortPtr_WithOffset(int cni, int cno, float* input, short* output, int length)
    {
        for (int i = 0; i < length; i++)
        {
            *output = (short)(*input * ConverterParams.CONVERT_FACTOR_SHORT);
            input += cni;
            output += cno;
        }
    }

    public static unsafe void ShortPtr_To_Float(ConverterParams context, short* input, long offset, long length, float[] output, long outOffset)
    {
        fixed (float* outputPtr = output)
        {
            ShortPtr_To_FloatPtr_WithOffset(
                context.SrcChannel.ChannelCount, context.DstChannel.ChannelCount, 
                input + offset * context.SrcChannel.ChannelCount + context.SrcChannel.Index,
                outputPtr + outOffset * context.DstChannel.ChannelCount + context.DstChannel.Index,
                (int)length);
        }
    }

    public static unsafe void ShortPtr_To_FloatPtr_WithOffset(int cni, int cno, short* input, float* output, int length)
    {
        for (int i = 0; i < length; i++)
        {
            *output = *input / ConverterParams.CONVERT_FACTOR_SHORT;
            input += cni;
            output += cno;
        }
    }
}
