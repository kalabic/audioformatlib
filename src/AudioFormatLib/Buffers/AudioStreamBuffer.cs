using AudioFormatLib.IO;
using AudioFormatLib.System;
using AudioFormatLib.Utils;
using System.Diagnostics;

namespace AudioFormatLib.Buffers;


/// <summary> WIP </summary>
public class AudioStreamBuffer : DisposableBuffer, IAudioBuffer
{
    public int AllocatedSize { get { return _buffer.MaxLength; } }

    public int AvailableSpace { get { return _buffer.MaxLength - _buffer.Count; } }

    public AFrameFormat Format { get { return _format; } }

    public bool IsClosed { get { return IsDisposed; } }

    public int StoredByteCount { get { return _buffer.Count; } }

    public IAudioInputs Input { get { return _inputs; } }

    public IAudioOutputs Output { get { return _outputs; } }



    private ABufferParams _bparams;

    private readonly AFrameFormat _format;

    private CancellableEventSlim _streamEvent;

    private IUnsafeBuffer _buffer;

    private AudioInputs _inputs;

    private AudioOutputs _outputs;

    protected int _bufferRequest = 0;

    public AudioStreamBuffer(ABufferParams bparams, CancellationToken cancellation)
    {
        Debug.Assert(bparams.BufferSize > 0);

        _bparams = bparams;
        _format = bparams.Format;
        _streamEvent = new CancellableEventSlim(cancellation);
        _buffer = AudioFrameTools.CreateUnsafeBuffer(bparams);
        _inputs = AudioFrameTools.CreateInputsWithBuffer(bparams, _buffer);
        _outputs = AudioFrameTools.CreateOutputsWithBuffer(bparams, _buffer);
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
