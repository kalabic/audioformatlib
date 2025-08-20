using System.Diagnostics;

namespace AudioFormatLib.Extensions;


/// <summary>
/// 
/// Extensions here allow for mixing different input and output types, Arrays and unsafe memory pointers.
/// 
/// </summary>
public readonly unsafe struct AFunction<ParamsT, ResultT>
{
    private readonly delegate*<AudioSpan, AudioSpan, ParamsT, ResultT> AudioSpanFunc;


    public AFunction(delegate*<AudioSpan, AudioSpan, ParamsT, ResultT> func)
    {
        Debug.Assert(func is not null);
        AudioSpanFunc = func;
    }


    public ResultT Process(in AudioSpan input, in AudioSpan output, ParamsT Params)
    {
        return AudioSpanFunc(input, output, Params);
    }


    public ResultT Process(in AudioPtr input, long offset, long length,
                           in AudioPtr output, long outOffset, long outLength,
                           ParamsT Params)
    {
        var inSpan = new AudioSpan(input, offset, length);
        var outSpan = new AudioSpan(output, outOffset, outLength);
        return AudioSpanFunc(inSpan, outSpan, Params);
    }


    public unsafe ResultT Process(byte* input, long offset, long length, AFrameFormat inputFormat,
                                  byte* output, long outOffset, long outLength, AFrameFormat outputFormat,
                                  ParamsT Params)
    {
        var inSpan = new AudioSpan(input, offset, length, inputFormat);
        var outSpan = new AudioSpan(output, outOffset, outLength, outputFormat);
        return AudioSpanFunc(inSpan, outSpan, Params);
    }


    /// <summary>
    /// 
    /// Because of implicit conversion operator in <see cref="AudioPtr.Any{T}"/> parameters
    /// <paramref name="input"/> and <paramref name="output"/> can serve as catch-all basket
    /// for different types of <see cref="Array"/> and unsafe memory pointer arguments.
    /// 
    /// </summary>
    public ResultT Process<IN, OUT>(in AudioPtr.Any<IN> input, long offset, long length, AFrameFormat inputFormat,
                                    in AudioPtr.Any<OUT> output, long outOffset, long outLength, AFrameFormat outputFormat,
                                    ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        Debug.Assert(inputFormat.SampleFormat == input.Format);
        Debug.Assert(outputFormat.SampleFormat == output.Format);

        using var fixedInput = input.MakeFixed();
        using var fixedOutput = output.MakeFixed();
        unsafe
        {
            var inputSpan = new AudioSpan((byte*)fixedInput, offset * sizeof(IN), length * sizeof(IN), inputFormat);
            var outputSpan = new AudioSpan((byte*)fixedOutput, outOffset * sizeof(OUT), outLength * sizeof(OUT), outputFormat);
            return AudioSpanFunc(inputSpan, outputSpan, Params);
        }
    }
}
