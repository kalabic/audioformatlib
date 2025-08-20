using AudioFormatLib.IO;
using System.Diagnostics;

namespace AudioFormatLib.Utils;


internal unsafe class SampleConsumer : IOutputConsumer<float>
{
    public long SamplesWritten { get { return _sampleCount; } }


    private readonly ConverterParams _params;

    private AudioSpan _consumer = AudioSpan.Empty;

    private long _sampleMaxCount;

    private long _sampleCount;


    public SampleConsumer(ConverterParams parameters)
    {
        _params = parameters;
    }

    public void SetOutput(in AudioSpan output)
    {
        _consumer = output;
        _sampleMaxCount = output.CountOf.Frames; // FRAME COUNT is always equivalent to count of samples inside ONE of channels.
        _sampleCount = 0;
    }

    public void ReleaseOutput()
    {
        _consumer = AudioSpan.Empty;
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
        int minLength = (int)Math.Min(length, spaceAvailable);

        if (minLength > 0)
        {
            fixed (float* inputPtr = array)
            {
                var input = new ASampleSpan<float>(inputPtr, offset, length).ByteSpan;
                var availableOutput = _consumer.GetFrameSpan(_sampleCount, minLength);
                _params.Func(_params, input, availableOutput);
                _sampleCount += minLength;
            }
        }
    }
}
