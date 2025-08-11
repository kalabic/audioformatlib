using System.Diagnostics;

namespace AudioFormatLib.Buffers;


public abstract class DisposableBuffer : IDisposable
{
    public bool IsDisposed { get { return _isDisposed != 0; } }

    private int _isDisposed;

    ~DisposableBuffer()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
#if DEBUG_UNDISPOSED
            Debug.Assert(false, "Object was not disposed properly.");
#endif
            Dispose(false);
        }
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Still safe invoke Dispose(), CloseBuffer(), etc. on managed objects.
        }
        /* else { danger zone for managed objects } */

        // - free unmanaged resources (unmanaged objects).
        // - safe to set large fields to null
    }
}
