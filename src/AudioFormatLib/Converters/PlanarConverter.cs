using AudioFormatLib.Utils;
using System.Diagnostics;

namespace AudioFormatLib.Converters;


/// <summary>
/// 
/// Convert sample values between planar source and destination PCM spans.
/// 
/// </summary>
public static class PlanarConverter
{
    public static unsafe readonly DelegateMap Mapping = new
    (
        new DelegateArray( &Float_To_Float, &Float_To_Short),
        new DelegateArray( &Short_To_Float, &Short_To_Short)
    );

    public static unsafe void Float_To_Float(in ConverterParams context,
                                             in AudioSpan input,
                                             in AudioSpan output)
    {
        Debug.Assert(input.CountOf.Samples <= output.CountOf.Samples);
        byte* offsetIn = (byte*)input.GetSamplePtr<float>(0, 0);
        byte* offsetOut = (byte*)output.GetSamplePtr<float>(0, 0);
        int length = (int)Math.Min(input.LengthSamples, output.LengthSamples);
        Buffer.MemoryCopy(offsetIn, offsetOut, output.Length, length * output.CountOf.BytesPerSampleFrame);
    }

    public static unsafe void Float_To_Short(in ConverterParams context,
                                             in AudioSpan input,
                                             in AudioSpan output)
    {
        Debug.Assert(input.CountOf.Samples <= output.CountOf.Samples);
        byte* offsetIn = (byte*)input.GetSamplePtr<float>(0, 0);
        byte* offsetOut = (byte*)output.GetSamplePtr<short>(0, 0);
        int length = (int)Math.Min(input.LengthSamples, output.LengthSamples);
        FloatPtr_To_ShortPtr_WithOffset(offsetIn, offsetOut, length);
    }

    public static unsafe void Short_To_Float(in ConverterParams context,
                                             in AudioSpan input,
                                             in AudioSpan output)
    {
        Debug.Assert(input.CountOf.Samples <= output.CountOf.Samples);
        byte* offsetIn = (byte*)input.GetSamplePtr<short>(0, 0);
        byte* offsetOut = (byte*)output.GetSamplePtr<float>(0, 0);
        int length = (int)Math.Min(input.LengthSamples, output.LengthSamples);
        ShortPtr_To_FloatPtr_WithOffset(offsetIn, offsetOut, length);
    }

    public static unsafe void Short_To_Short(in ConverterParams context,
                                             in AudioSpan input,
                                             in AudioSpan output)
    {
        Debug.Assert(input.CountOf.Samples <= output.CountOf.Samples);
        byte* offsetIn = (byte*)input.GetSamplePtr<short>(0, 0);
        byte* offsetOut = (byte*)output.GetSamplePtr<short>(0, 0);
        int length = (int)Math.Min(input.LengthSamples, output.LengthSamples);
        Buffer.MemoryCopy(offsetIn, offsetOut, output.Length, length * output.CountOf.BytesPerSampleFrame);
    }

    public static unsafe void Float_To_ShortPtr(ConverterParams context, float[] input, long offset, long length, byte* output, long outOffset)
    {
        fixed (float* inputPtr = input)
        {
            FloatPtr_To_ShortPtr(context, (byte*)inputPtr, offset, length, output, outOffset);
        }
    }

    public static unsafe void FloatPtr_To_ShortPtr(ConverterParams context, byte* input, long offset, long length, byte* output, long outOffset)
    {
        float* offsetInputPtr = (float*)input + offset;
        short* offsetOutputPtr = (short*)output + outOffset;
        FloatPtr_To_ShortPtr_WithOffset(
            (byte*)offsetInputPtr, (byte*)offsetOutputPtr, (int)length);
    }

    public static unsafe void FloatPtr_To_ShortPtr_WithOffset(byte* input, byte* output, int length)
    {
        float* floatIn = (float*)input;
        short* shortOut = (short*)output;
        for (int i = 0; i < length; i++)
        {
            *shortOut++ = (short)(*floatIn++ * ConverterParams.CONVERT_FACTOR_SHORT);
        }
    }

    public static unsafe void ShortPtr_To_Float(ConverterParams context, byte* input, long offset, long length, float[] output, long outOffset)
    {
        fixed (float* outputPtr = output)
        {
            short* offsetInputPtr = (short*)input + offset;
            float* offsetOutputPtr = outputPtr + outOffset;
            ShortPtr_To_FloatPtr_WithOffset(
                (byte*)offsetInputPtr, (byte*)offsetOutputPtr, (int)length);
        }
    }

    public static unsafe void ShortPtr_To_FloatPtr_WithOffset(byte* input, byte* output, int length)
    {
        short* shortIn = (short*)input;
        float* floatOut = (float*)output;
        for (int i = 0; i < length; i++)
        {
            *floatOut++ = *shortIn++ / ConverterParams.CONVERT_FACTOR_SHORT;
        }
    }
}
