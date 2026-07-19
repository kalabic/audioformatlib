using AudioFormatLib.IO;
using DotBase.Buffers;
using DotBase.Cancellation;
using DotBase.Core;
using System.Diagnostics;

namespace AudioFormatLib.Buffers;


/// <summary> WIP </summary>
public class AudioStreamBuffer : DisposableBase, IAudioBuffer
{
    public static AudioStreamBuffer CreateForDuration(
        APcmFormat format,
        TimeSpan capacity,
        bool waitForCompleteRead = false,
        CancellationToken cancellationToken = default)
    {
        if (format.SampleValueFormat == ASampleValueFormat.NONE ||
            format.SampleRate <= 0 ||
            format.ChannelCount <= 0 ||
            format.BytesPerSampleFrame <= 0)
        {
            throw new ArgumentException("A complete PCM format is required.", nameof(format));
        }

        if (capacity <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Buffer duration must be positive.");
        }

        decimal sampleCount = decimal.Ceiling(
            (decimal)format.SampleRate * capacity.Ticks / TimeSpan.TicksPerSecond);
        decimal byteCount = sampleCount * format.BytesPerSampleFrame;
        if (byteCount > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "The requested buffer is too large.");
        }

        return new AudioStreamBuffer(
            new ABufferParams
            {
                Format = format,
                BufferSize = decimal.ToInt32(byteCount),
                WaitForCompleteRead = waitForCompleteRead
            },
            cancellationToken);
    }

    public int AllocatedSize { get { return _buffer.Capacity; } }

    public int AvailableSpace { get { return _buffer.Capacity - _buffer.Count; } }

    public APcmFormat Format { get { return _format; } }

    public bool IsClosed { get { return IsDisposed; } }

    public int StoredByteCount { get { return _buffer.Count; } }

    /// <summary> Total scalar sample values stored across all channels. </summary>
    public int StoredSampleValueCount { get { return _buffer.Count / _format.SampleValueFormat.Size(); } }

    /// <summary> Temporal samples stored; equivalently, samples stored in each channel. </summary>
    public int StoredSampleCount { get { return _buffer.Count / _format.BytesPerSampleFrame; } }

    public IAudioInputs Input { get { return _inputs; } }

    public IAudioOutputs Output { get { return _outputs; } }



    private ABufferParams _bparams;

    private readonly APcmFormat _format;

    private CancellableEventSlim _streamEvent;

    private IByteRingBuffer _buffer;

    private AudioInputs _inputs;

    private AudioOutputs _outputs;

    protected int _bufferRequest = 0;

    public AudioStreamBuffer(ABufferParams bparams, CancellationToken cancellation)
    {
        Debug.Assert(bparams.BufferSize > 0);

        _bparams = bparams;
        _format = bparams.Format;
        _streamEvent = new CancellableEventSlim(cancellation);
        _buffer = ATools.CreateUnsafeBuffer(bparams);
        _inputs = ATools.CreateInputsWithBuffer(bparams, _buffer);
        _outputs = ATools.CreateOutputsWithBuffer(bparams, _buffer);
    }

    public AudioStreamBuffer(ABufferParams bufferParams)
        : this(bufferParams, CancellationToken.None)
    { }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CloseBuffer();
            _streamEvent.Dispose();
        }

        base.Dispose(disposing);
    }

    public virtual void CloseBuffer()
    {
        _buffer.Close();
        _streamEvent.Cancel(IsDisposed);
    }

    public void ClearBuffer()
    {
        _buffer.ClearBuffer();
    }
}
