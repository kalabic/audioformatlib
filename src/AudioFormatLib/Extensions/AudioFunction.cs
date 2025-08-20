using AudioFormatLib.Buffers;
using System.Diagnostics;

namespace AudioFormatLib.Extensions;


/// <summary>
/// 
/// Extensions here allow for mixing different input and output types, Arrays and unsafe memory pointers.
/// 
/// </summary>
public abstract class AudioFunction<ParamsT, ResultT>
    : DisposableBuffer
{
    public AFrameFormat InputFormat { get { return _inputFormat; } }

    public AFrameFormat OutputFormat { get { return _outputFormat; } }


    private AFrameFormat _inputFormat;

    private AFrameFormat _outputFormat;


    protected AudioFunction(in AFrameFormat input, in AFrameFormat output)
        : base()
    {
        Debug.Assert(input.NumChannels == output.NumChannels); // WIP
        _inputFormat = input;
        _outputFormat = output;
    }


    public abstract ResultT Process(in AudioSpan input, in AudioSpan output, ParamsT Params);


    public ResultT Process(in AudioPtr input, long offset, long length,
                           in AudioPtr output, long outOffset, long outLength,
                           ParamsT Params)
    {
        var inSpan = new AudioSpan(input, offset, length);
        var outSpan = new AudioSpan(output, outOffset, outLength);
        return Process(inSpan, outSpan, Params);
    }


    public unsafe ResultT Process(byte* input, long offset, long length,
                                  byte* output, long outOffset, long outLength,
                                  ParamsT Params)
    {
        var inSpan = new AudioSpan(input, offset, length, _inputFormat);
        var outSpan = new AudioSpan(output, outOffset, outLength, _outputFormat);
        return Process(inSpan, outSpan, Params);
    }


    /// <summary>
    /// 
    /// Because of implicit conversion operator in <see cref="AudioPtr.Any{T}"/> parameters
    /// <paramref name="input"/> and <paramref name="output"/> can serve as catch-all basket
    /// for different types of <see cref="Array"/> and unsafe memory pointer arguments.
    /// 
    /// </summary>
    public ResultT Process<IN, OUT>(in AudioPtr.Any<IN> input, long offset, long length,
                                    in AudioPtr.Any<OUT> output, long outOffset, long outLength,
                                    ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        Debug.Assert(_inputFormat.SampleFormat == input.Format);
        Debug.Assert(_outputFormat.SampleFormat == output.Format);

        using var fixedInput = input.MakeFixed();
        using var fixedOutput = output.MakeFixed();
        unsafe
        {
            var inputSpan = new AudioSpan((byte*)fixedInput, offset * sizeof(IN), length * sizeof(IN), _inputFormat);
            var outputSpan = new AudioSpan((byte*)fixedOutput, outOffset * sizeof(OUT), outLength * sizeof(OUT), _outputFormat);
            return Process(inputSpan, outputSpan, Params);
        }
    }
}
