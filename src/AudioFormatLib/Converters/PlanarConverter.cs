using AudioFormatLib.Utils;

namespace AudioFormatLib.Converters;


/// <summary>
/// 
/// Treat data inside source and destination frames like they are a simple single-channel audio.
/// 
/// </summary>
public static class PlanarConverter
{
    public static unsafe void Float_To_ShortPtr(ConverterParams context, float[] input, long offset, long length, short* output, long outOffset)
    {
        fixed (float* inputPtr = input)
        {
            FloatPtr_To_ShortPtr_WithOffset(inputPtr + offset, output + outOffset + context.DstChannel.Index, (int)length);
        }
    }

    public static unsafe void FloatPtr_To_ShortPtr_WithOffset(float* input, short* output, int length)
    {
        for (int i = 0; i < length; i++)
        {
            *output++ = (short)(*input++ * ConverterParams.CONVERT_FACTOR_SHORT);
        }
    }

    public static unsafe void ShortPtr_To_Float(ConverterParams context, short* input, long offset, long length, float[] output, long outOffset)
    {
        fixed (float* outputPtr = output)
        {
            ShortPtr_To_FloatPtr_WithOffset(input + offset + context.SrcChannel.Index, outputPtr + outOffset, (int)length);
        }
    }

    public static unsafe void ShortPtr_To_FloatPtr_WithOffset(short* input, float* output, int length)
    {
        for (int i = 0; i < length; i++)
        {
            *output++ = *input++ / ConverterParams.CONVERT_FACTOR_SHORT;
        }
    }
}
