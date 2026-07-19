using AudioFormatLib.IO;
using System.Diagnostics;

namespace AudioFormatLib.Utils;


internal unsafe class SampleValueConsumer : IOutputConsumer<float>
{
    public long SampleValuesWritten { get { return _sampleValueCount; } }


    private readonly ConverterParams _params;

    private AudioSpan _consumer = AudioSpan.Empty;

    private long _sampleValueCapacity;

    private long _sampleValueCount;


    public SampleValueConsumer(ConverterParams parameters)
    {
        _params = parameters;
    }

    public void SetOutput(in AudioSpan output)
    {
        _consumer = output;
        _sampleValueCapacity = output.CountOf.Samples;
        _sampleValueCount = 0;
    }

    public void ReleaseOutput()
    {
        _consumer = AudioSpan.Empty;
        _sampleValueCapacity = 0;
        _sampleValueCount = 0;
    }

    public long GetOutputBufferLength()
    {
        return _sampleValueCapacity - _sampleValueCount;
    }

    public void ConsumeOutput(float[] array, int sampleValueOffset, int sampleValueCount)
    {
        long spaceAvailable = _sampleValueCapacity - _sampleValueCount;
        Debug.Assert(sampleValueCount <= spaceAvailable, "Cannot consume more sample values than space available.");
        int minLength = (int)Math.Min(sampleValueCount, spaceAvailable);

        if (minLength > 0)
        {
            fixed (float* inputPtr = array)
            {
                var input = new ASampleValueSpan<float>(
                    inputPtr,
                    sampleValueOffset,
                    sampleValueCount).ByteSpan;
                var availableOutput = _consumer.GetSampleSpan(_sampleValueCount, minLength);
                _params.Func(_params, input, availableOutput);
                _sampleValueCount += minLength;
            }
        }
    }
}
