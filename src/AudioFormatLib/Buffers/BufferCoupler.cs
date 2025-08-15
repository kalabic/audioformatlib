using AudioFormatLib.Extensions;
using AudioFormatLib.IO;
using AudioFormatLib.Utils;
using System.Diagnostics;

namespace AudioFormatLib.Buffers;


public unsafe class BufferCoupler : DisposableBuffer
{
    private const int IO_DETACHED = 0;

    private const int IO_ATTACHED = 1;

    private SampleProducer[] _producers;

    private SampleConsumer[] _consumers;

    private int _ioState;

    private readonly int _numChannels;

    private readonly int _inputSampleSize;

    private readonly int _outputSampleSize;



    public BufferCoupler(int numChannels, int inputSampleSize, int outputSampleSize)
    {
        _numChannels = numChannels;
        _inputSampleSize = inputSampleSize;
        _outputSampleSize = outputSampleSize;

        _producers = new SampleProducer[_numChannels];
        _consumers = new SampleConsumer[_numChannels];
        for (int i = 0; i < _producers.Length; i++)
        {
            _producers[i] = AudioFrameTools.CreateSampleProducer(new AChannelId(i, _numChannels));
            _consumers[i] = AudioFrameTools.CreateSampleConsumer(new AChannelId(i, _numChannels));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            int ioState = Volatile.Read(ref _ioState);
            if (ioState == IO_ATTACHED)
            {
                Debug.Assert(false, "Being disposed with fully attached IO. (Bad)");
                ReleaseBuffers();
            }

            _producers = [];
            _consumers = [];
        }
        base.Dispose(disposing);
    }

    internal IInputProducer<float> GetProducer(int channelIndex)
    {
        return _producers[channelIndex];
    }

    internal IOutputConsumer<float> GetConsumer(int channelIndex)
    {
        return _consumers[channelIndex];
    }

    public bool ReleaseBuffers()
    {
        bool isAttached = Volatile.Read(ref _ioState) != IO_DETACHED;
        Debug.Assert(isAttached, "Buffers not attached.");
        if (isAttached)
        {
            for (int i = 0; i < _consumers.Length; i++)
            {
                _consumers[i].ReleaseOutput();
                _producers[i].ReleaseInput();
            }
            Volatile.Write(ref _ioState, IO_DETACHED);
        }
        return isAttached;
    }

    public unsafe bool AttachBuffers<IN, OUT>(IN* input, long inputLength, OUT* output, long outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        bool wasNotAttached = Interlocked.CompareExchange(ref _ioState, IO_ATTACHED, IO_DETACHED) == IO_DETACHED;
        Debug.Assert(wasNotAttached, "Buffers already attached.");
        if (wasNotAttached)
        {
            PrepareInput(input, inputLength);
            PrepareOutput(output, outputLength);
        }
        return wasNotAttached;
    }

    /// <summary>
    /// 
    /// 'byte[]' and 'byte*' parameters provide 'length' as byte-size length of a complete audio frame (or buffer).
    /// Other types need to provide 'length' as a number of samples.
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    private void PrepareByteInput(byte* value, long length)
    {
        long sampleCount = length / _inputSampleSize;
        Debug.Assert(sampleCount % _numChannels == 0, "Input buffer does not contain complete audio frame. (Or number of channels in wrong)");

        for (int i = 0; i < _producers.Length; i++)
        {
            _producers[i].SetInput(value, sampleCount / _numChannels);
        }
    }

    private void PrepareInput<T>(T* value, long length) where T : unmanaged
    {
        if (GenericType<T>.IsByte)
        {
            PrepareByteInput((byte*)value, length);
            return;
        }

        Debug.Assert(length % _numChannels == 0, "Input buffer does not contain complete audio frame. (Or number of channels in wrong)");
        Debug.Assert(sizeof(T) == _inputSampleSize, "Declared sample size at input does not correspond to this type.");

        for (int i = 0; i < _producers.Length; i++)
        {
            _producers[i].SetInput(value, length / _numChannels);
        }
    }

    /// <summary>
    /// 
    /// 'byte[]' and 'byte*' parameters provide 'length' as byte-size length of a complete audio frame (or buffer).
    /// Other types need to provide 'length' as a number of samples.
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    private void PrepareByteOutput(byte* value, long length)
    {
        long sampleCount = length / _outputSampleSize;
        for (int i = 0; i < _consumers.Length; i++)
        {
            _consumers[i].SetOutput(value, sampleCount / _numChannels);
        }
    }

    private void PrepareOutput<T>(T* value, long length) where T : unmanaged
    {
        if (GenericType<T>.IsByte)
        {
            PrepareByteOutput((byte*)value, length);
            return;
        }

        Debug.Assert(sizeof(T) == _outputSampleSize, "Declared sample size at output does not correspond to this type.");

        for (int i = 0; i < _consumers.Length; i++)
        {
            _consumers[i].SetOutput(value, length / _numChannels);
        }
    }
}
