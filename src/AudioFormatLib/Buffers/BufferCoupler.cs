using AudioFormatLib.IO;
using AudioFormatLib.Utils;
using DotBase.Core;
using System.Diagnostics;

namespace AudioFormatLib.Buffers;


public unsafe class BufferCoupler : DisposableBase
{
    public readonly int ChannelCount;

    public readonly APcmFormat InputFormat;

    public readonly APcmFormat OutputFormat;



    private const int IO_DETACHED = 0;

    private const int IO_ATTACHED = 1;

    private SampleValueProducer[] _producers;

    private SampleValueConsumer[] _consumers;

    private int _ioState;

    public BufferCoupler(APcmFormat input, APcmFormat output)
    {
        Debug.Assert(input.ChannelLayout.Count == output.ChannelLayout.Count); // WIP
        ChannelCount = input.ChannelLayout.Count;
        InputFormat = input;
        OutputFormat = output;

        _producers = new SampleValueProducer[ChannelCount];
        _consumers = new SampleValueConsumer[ChannelCount];
        for (int i = 0; i < _producers.Length; i++)
        {
            _producers[i] = ATools.CreateSampleValueProducer(new AChannelId(i, ChannelCount));
            _consumers[i] = ATools.CreateSampleValueConsumer(new AChannelId(i, ChannelCount));
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

    public unsafe bool AttachBuffers(in AudioSpan input, in AudioSpan output)
    {
        bool wasNotAttached = Interlocked.CompareExchange(ref _ioState, IO_ATTACHED, IO_DETACHED) == IO_DETACHED;
        Debug.Assert(wasNotAttached, "Buffers already attached.");
        if (wasNotAttached)
        {
            PrepareInput(input);
            PrepareOutput(output);
        }
        return wasNotAttached;
    }

    private void PrepareInput(in AudioSpan input)
    {
        for (int i = 0; i < _producers.Length; i++)
        {
            _producers[i].SetInput(input);
        }
    }

    private void PrepareOutput(in AudioSpan output)
    {
        for (int i = 0; i < _consumers.Length; i++)
        {
            _consumers[i].SetOutput(output);
        }
    }
}
