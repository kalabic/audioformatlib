using AudioFormatLib.IO;
using System.Diagnostics;

namespace AudioFormatLib.Utils;


internal unsafe class SampleProducer : IInputProducer<float>
{
    public long SamplesRead { get { return _samplesUsed; } }


    private readonly ConverterParams _params;

    private AudioSpan _producer = AudioSpan.Empty;

    private long _sampleCount = 0;

    private long _samplesUsed = 0;


    public SampleProducer(ConverterParams parameters)
    {
        _params = parameters;
    }

    public void SetInput(in AudioSpan input)
    {
        Debug.Assert(_samplesUsed == _sampleCount, "Previous operation did not use all of stored samples.");
        _producer = input;
        _sampleCount = input.CountOf.Frames; // FRAME COUNT is always equivalent to count of samples inside ONE of channels.
        _samplesUsed = 0;
    }

    public void ReleaseInput()
    {
        _producer = AudioSpan.Empty;
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
        Debug.Assert(length <= samplesAvailable, "Cannot produce more samples than there are samples stored.");

        int minLength = (int)Math.Min(length, samplesAvailable);
        if (minLength > 0)
        {
            var producerSubSpan = _producer.GetFrameSpan(_samplesUsed, minLength);

            fixed (float* outputPtr = array)
            {
                var outputSpan = new ASampleSpan<float>(outputPtr, offset, length).ByteSpan;
                _params.Func(_params, producerSubSpan, outputSpan);
                _samplesUsed += minLength;
            }
        }
    }
}
