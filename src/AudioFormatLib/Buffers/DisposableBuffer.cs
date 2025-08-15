using System.Diagnostics;

namespace AudioFormatLib.Buffers;


public abstract class DisposableBuffer : IDisposable
{
    public enum STATUS : int
    {
        NOT_DISPOSED,
        DISPOSED,
        FINALIZED
    }

    public bool IsDisposed { get { return _isDisposed != 0; } }

    public STATUS DisposeStatus { get { return _status; } }


    private int _isDisposed;

    private STATUS _status = STATUS.NOT_DISPOSED;

    ~DisposableBuffer()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
#if DEBUG_UNDISPOSED
            Debug.Assert(false, "Object was not disposed properly.");
#endif
            Dispose(false);
            Debug.Assert(_status == STATUS.FINALIZED, "Overridden dispose method not invoked. Is this what you wanted?");
        }
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            Dispose(true);
            Debug.Assert(_status == STATUS.DISPOSED, "Overridden dispose method not invoked. Is this what you wanted?");
            GC.SuppressFinalize(this);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Still safe to invoke Dispose(), CloseBuffer(), etc. on managed objects.
            _status = STATUS.DISPOSED;
        }
        else
        {
            // Danger zone for managed objects.
            _status = STATUS.FINALIZED;
        }
    }
}
