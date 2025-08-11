using AudioFormatLib.IO;
using AudioFormatLib.System;
using System.Diagnostics;

namespace AudioFormatLib.Buffers;


/// <summary> WIP </summary>
public class AudioStreamBuffer : DisposableBuffer, IAudioBuffer
{
    public int AllocatedSize { get { return _buffer.MaxLength; } }

    public int AvailableSpace { get { return _buffer.MaxLength - _buffer.Count; } }

    public bool IsClosed { get { return IsDisposed; } }

    public int StoredByteCount { get { return _buffer.Count; } }


    private ABufferParams _bparams;

    private CancellableEventSlim _streamEvent;

    private IUnsafeBuffer _buffer;

    private SharedAudioBufferInput _input;

    private SharedAudioBufferOutput _output;

    protected int _bufferRequest = 0;

    public AudioStreamBuffer(ABufferParams bparams, CancellationToken cancellation)
    {
        Debug.Assert(bparams.BufferSize > 0);

        _bparams = bparams;
        _streamEvent = new CancellableEventSlim(cancellation);
        if (bparams.WaitForCompleteRead)
        {
            _buffer = new CircularBufferWaitable(bparams.BufferSize);
        }
        else
        {
            _buffer = new CircularBufferLocked(bparams.BufferSize);
        }
        _input = new SharedAudioBufferInput(_buffer);
        _output = new SharedAudioBufferOutput(_buffer);
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

    public IAudioBufferInput GetBufferInput()
    {
        return _input;
    }

    public IAudioBufferOutput GetBufferOutput()
    {
        return _output;
    }

    public IAudioStreamInput GetStreamInput()
    {
        return _input;
    }

    public IAudioStreamOutput GetStreamOutput()
    {
        return _output;
    }

    public virtual void CloseBuffer()
    {
        _buffer.Close();
        _streamEvent.Cancel(IsDisposed);
    }

    public void ClearBuffer()
    {
        _buffer.Reset();
    }
}
