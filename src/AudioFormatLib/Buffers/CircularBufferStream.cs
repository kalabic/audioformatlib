using AudioFormatLib.System;
using AudioFormatLib.Utils;
using System.Text;

namespace AudioFormatLib.Buffers;

/// <summary> WIP </summary>
public class CircularBufferStream : Stream, IStreamBuffer
{
    public bool IsCancelled { get { return _streamEvent.IsEventCancelled; } }

    public bool IsClosed { get { return _streamBuffer is null; } }

    private readonly object _lockObject;

    private CancellableEventSlim _streamEvent;

    private CircularBuffer? _streamBuffer = null;

    protected long _totalBytesWritten = 0;

    protected int _bufferRequest = 0;

    protected bool _disposed = false;

    public CircularBufferStream(int bufferSize, CancellationToken cancellation)
    {
        _lockObject = new object();
        _streamEvent = new CancellableEventSlim(cancellation);
        _streamBuffer = new CircularBuffer(bufferSize);
    }

    public CircularBufferStream(int bufferSize)
        : this(bufferSize, CancellationToken.None)
    { }

    protected override void Dispose(bool disposing)
    {
        // Release managed resources.
        if (disposing)
        {
            lock (_lockObject)
            {
                _disposed = true;
                if (_streamBuffer is not null)
                {
                    _streamBuffer.Reset();
                    _streamBuffer = null;
                }
                _streamEvent.Dispose();
            }
        }

        // Release unmanaged resources.
        base.Dispose(disposing);
    }

    public void Cancel()
    {
        _streamEvent.Cancel();
    }

    public override void Close()
    {
        base.Close();
        lock(_lockObject)
        {
            if (_streamBuffer is not null)
            {
                _streamBuffer.Reset();
                _streamBuffer = null;
            }
        }
    }

    public void Write(byte[] buffer)
    {
        Write(buffer, 0, buffer.Length);
    }

    public void Write(string text)
    {
        if (!String.IsNullOrEmpty(text))
        {
            Write(Encoding.UTF8.GetBytes(text));
        }
    }

    public void ClearBuffer()
    {
        lock (_lockObject)
        {
            _streamBuffer?.Reset();
            _totalBytesWritten = 0;

            // Temporary workaround for propagating clear buffer request.
            SetBufferRequest(1);
        }
    }

    public int GetBufferRequest()
    {
        int req = _bufferRequest;
        _bufferRequest = 0;
        return req;
    }

    public void SetBufferRequest(int value)
    {
        _bufferRequest = value;
    }

    public int GetBufferedBytes()
    {
        return (_streamBuffer is not null && !IsCancelled) ? _streamBuffer.Count : 0;
    }

    public bool WaitDataAvailable(int minAvailable, int timeoutMs = 0, bool noExceptions = true)
    {
        int available = GetBytesAvailable(minAvailable);
        if (available >= minAvailable)
        {
            return true;
        }
        else if (available >= 0)
        {
            while (_streamEvent.Wait(noExceptions))
            {
                available = GetBytesAvailable(minAvailable);
                if (available >= minAvailable)
                {
                    return true;
                }
                else if (available < 0)
                {
                    // _streamBuffer is null
                    return false;
                }
            }
        }

        return false;
    }

    public int GetBytesAvailable()
    {
        lock (_lockObject)
        {
            if (!IsCancelled)
            {
                return (_streamBuffer is not null) ? _streamBuffer.Count : -1;
            }
            else
            {
                return 0;
            }
        }
    }

    public int GetBytesUnused()
    {
        lock (_lockObject)
        {
            if (!IsCancelled)
            {
                return (_streamBuffer is not null) ? (_streamBuffer.MaxLength - _streamBuffer.Count) : -1;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// 
    /// If parameter <paramref name="minAvailable"/> is 0, this function will not reset internal
    /// synchronization event object that is used by waitable function here like 
    /// <see cref="CircularBufferStream.WaitDataAvailable(int, int, bool)"/>.
    /// 
    /// However if set to value larger than 0, it will reset that event in case availabe data is
    /// smaller than given value.
    /// 
    /// </summary>
    /// <param name="minAvailable"></param>
    /// <returns></returns>
    virtual protected int GetBytesAvailable(int minAvailable)
    {
        lock (_lockObject)
        {
            if (!IsCancelled)
            {
                int available = (_streamBuffer is not null) ? _streamBuffer.Count : -1;
                if ((available >= 0) && (available < minAvailable))
                {
                    _streamEvent.Reset();
                }
                return available;
            }
            else
            {
                return 0;
            }
        }
    }

    public override bool CanRead => (_streamBuffer is not null && !IsCancelled);

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanWrite => (_streamBuffer is not null && !IsCancelled);

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        lock (_lockObject)
        {
            if (_streamBuffer is not null && !IsCancelled)
            {
                int bytesRead = _streamBuffer.Read(buffer, offset, count);
                if (_streamBuffer.Count == 0)
                {
                    _streamEvent.Reset();
                }
                return bytesRead;
            }
        }
        return 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        lock (_lockObject)
        {
            if (_streamBuffer is not null && !IsCancelled)
            {
                _totalBytesWritten += _streamBuffer.Write(buffer, offset, count);
                _streamEvent.Set();
            }
        }
    }

    public int MovePacket(IStreamBuffer other, byte[] buffer)
    {
        int bytesRead = 0;

        lock (_lockObject)
        {
            if (_streamBuffer is not null && 
                !IsCancelled &&
                _streamBuffer.Count >= buffer.Length)
            {
                bytesRead = _streamBuffer.Read(buffer, 0, buffer.Length);
                if (_streamBuffer.Count == 0)
                {
                    _streamEvent.Reset();
                }
            }
        }

        if (bytesRead == buffer.Length)
        {
            other.GetOutputStream().Write(buffer, 0, buffer.Length);
            return bytesRead;
        }

        return 0;
    }

    public Stream GetInputStream()
    {
        return this;
    }

    public Stream GetOutputStream()
    {
        return this;
    }
}
