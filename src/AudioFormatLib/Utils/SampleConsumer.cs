using AudioFormatLib.IO;
using System.Diagnostics;

namespace AudioFormatLib.Utils;


internal unsafe class SampleConsumer : IOutputConsumer<float>
{
    public long SamplesWritten { get { return _sampleCount; } }


    private readonly ConverterParams _params;

    private readonly AConversion_Float_To_ShortPtr _function;

    private byte* _output = null;

    private long _sampleMaxCount = 0;

    private long _sampleCount = 0;


    public SampleConsumer(ConverterParams parameters, AConversion_Float_To_ShortPtr function)
    {
        _params = parameters;
        _function = function;
    }

    public void SetOutput<T>(T* ptr, long count) where T : unmanaged
    {
        Debug.Assert(ptr != null && count > 0, "Null or no output provided.");
        _output = (byte *)ptr;
        _sampleMaxCount = count;
        _sampleCount = 0;
    }

    public void ReleaseOutput()
    {
        _output = null;
        _sampleMaxCount = 0;
        _sampleCount = 0;
    }

    public long GetOutputBufferLength()
    {
        return _sampleMaxCount - _sampleCount;
    }

    public void ConsumeOutput(float[] array, int offset, int length)
    {
        long spaceAvailable = _sampleMaxCount - _sampleCount;
        Debug.Assert(length <= spaceAvailable, "Cannot consume more samples than space available.");
        int minLength = (int)Math.Min((long)length, spaceAvailable);
        _function(_params, array, offset, minLength, _output, _sampleCount);
        _sampleCount += minLength;
    }
}
