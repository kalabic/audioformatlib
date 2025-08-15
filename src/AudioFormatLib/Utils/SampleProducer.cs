using AudioFormatLib.IO;
using System.Diagnostics;

namespace AudioFormatLib.Utils;


internal unsafe class SampleProducer : IInputProducer<float>
{
    public long SamplesRead { get { return _samplesUsed; } }


    private readonly ConverterParams _params;

    private readonly AConversion_ShortPtr_To_Float _function;

    private byte* _input = null;

    private long _sampleCount = 0;

    private long _samplesUsed = 0;


    public SampleProducer(ConverterParams parameters, AConversion_ShortPtr_To_Float function)
    {
        _params = parameters;
        _function = function;
    }

    public void SetInput<T>(T* ptr, long count) where T : unmanaged
    {
        SetInputBytes((byte*)ptr, count);
    }

    private void SetInputBytes(byte* ptr, long count)
    {
        Debug.Assert(ptr != null && count > 0, "Null or no input provided.");
        Debug.Assert(_samplesUsed == _sampleCount, "Previous operation did not use all of input.");
        _input = (byte*)ptr;
        _sampleCount = count;
        _samplesUsed = 0;
    }

    public void ReleaseInput()
    {
        _input = null;
        _sampleCount = 0;
        _samplesUsed = 0;
    }

    public long GetInputBufferLength()
    {
        return _sampleCount - _samplesUsed;
    }

    public void ProduceInput(float[] array, int offset, int length)
    {
        long samplesAvailable = _sampleCount - _samplesUsed;
        Debug.Assert(length <= samplesAvailable, "Cannot produce more samples than stored.");
        int minLength = (int)Math.Min((long)length, samplesAvailable);
        _function(_params, _input, _samplesUsed, minLength, array, offset);
        _samplesUsed += minLength;
    }
}
