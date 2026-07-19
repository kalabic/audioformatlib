using AudioFormatLib.IO;
using System.Diagnostics;

namespace AudioFormatLib.Utils;


internal unsafe class SampleValueProducer : IInputProducer<float>
{
    public long SampleValuesRead { get { return _sampleValuesUsed; } }


    private readonly ConverterParams _params;

    private AudioSpan _producer = AudioSpan.Empty;

    private long _sampleValueCount = 0;

    private long _sampleValuesUsed = 0;


    public SampleValueProducer(ConverterParams parameters)
    {
        _params = parameters;
    }

    public void SetInput(in AudioSpan input)
    {
        Debug.Assert(
            _sampleValuesUsed == _sampleValueCount,
            "Previous operation did not use all stored sample values.");
        _producer = input;
        _sampleValueCount = input.CountOf.Samples;
        _sampleValuesUsed = 0;
    }

    public void ReleaseInput()
    {
        _producer = AudioSpan.Empty;
        _sampleValueCount = 0;
        _sampleValuesUsed = 0;
    }

    public long GetInputBufferLength()
    {
        return _sampleValueCount - _sampleValuesUsed;
    }

    public void ProduceInput(float[] array, int sampleValueOffset, int sampleValueCount)
    {
        long sampleValuesAvailable = _sampleValueCount - _sampleValuesUsed;
        Debug.Assert(
            sampleValueCount <= sampleValuesAvailable,
            "Cannot produce more sample values than are stored.");

        int minLength = (int)Math.Min(sampleValueCount, sampleValuesAvailable);
        if (minLength > 0)
        {
            var producerSubSpan = _producer.GetSampleSpan(_sampleValuesUsed, minLength);

            fixed (float* outputPtr = array)
            {
                var outputSpan = new ASampleValueSpan<float>(
                    outputPtr,
                    sampleValueOffset,
                    sampleValueCount).ByteSpan;
                _params.Func(_params, producerSubSpan, outputSpan);
                _sampleValuesUsed += minLength;
            }
        }
    }
}
