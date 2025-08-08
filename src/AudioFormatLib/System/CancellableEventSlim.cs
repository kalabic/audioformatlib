using System.Diagnostics;

namespace AudioFormatLib.System;



/// <summary>
/// 
/// <see cref="CancellableEventSlim.Wait"/> is cancellable both by <see cref="CancellationToken"/>
/// provided in constructor and its own unrelated method <see cref="CancellableEventSlim.Cancel"/>
/// 
/// </summary>
public class CancellableEventSlim : IDisposable
{
    public event EventHandler<Exception>? ExceptionOccured;

    public bool IsDisposed { get { return _isDisposed != 0; } }

    public bool IsEventCancelled { get {  return _cancellation.IsCancellationRequested || (_cancellationEvent != 0); } }

    public bool IsUsable { get { return !_cancellation.IsCancellationRequested && (_cancellationEvent == 0) && (_isDisposed == 0); } }



    private int _isDisposed;

    private Object _lock = new();

    private CancellationToken _cancellation;

    private int _cancellationEvent;

    private ManualResetEventSlim _event = new ManualResetEventSlim(false, 0);



    public CancellableEventSlim(CancellationToken cancellation)
    {
        Debug.Assert(!cancellation.IsCancellationRequested);
        _cancellation = cancellation;
    }

    public void Dispose()
    {
        lock(_lock)
        {
            if (IsDisposed)
            {
                return;
            }

            _isDisposed = 1;
            _cancellationEvent = 1;
            _event.Dispose();
        }
    }

    public void Cancel()
    {
        lock (_lock)
        {
            if (!IsDisposed)
            {
                _cancellationEvent = 1;
                _event.Set();
            }
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            if (IsDisposed)
            {
                return;
            }

            if (IsEventCancelled)
            {
                Debug.Assert(_event.IsSet);
                return;
            }

            _event.Reset();
            return;
        }
    }

    public void Set()
    {
        lock (_lock)
        {
            if (IsDisposed)
            {
                return;
            }

            if (IsEventCancelled)
            {
                Debug.Assert(_event.IsSet);
                return;
            }

            _event.Set();
            return;
        }
    }

    /// <summary>
    /// 
    /// Is cancellable both by <see cref="CancellationToken"/> provided in constructor and <see cref="CancellableEventSlim.Cancel"/>.
    /// 
    /// </summary>
    /// <param name="noExceptions"></param>
    /// <returns></returns>
    public bool Wait(bool noExceptions = true)
    {
        if (IsDisposed)
        {
            return false;
        }

        if (IsEventCancelled)
        {
            Debug.Assert(_event.IsSet);
            return false;
        }

        if (noExceptions)
        {
            try
            {
                _event.Wait(_cancellation);
            }
            catch (OperationCanceledException ex)
            {
                ExceptionOccured?.Invoke(this, ex);
                return false;
            }
            catch (ObjectDisposedException ex)
            {
                ExceptionOccured?.Invoke(this, ex);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                ExceptionOccured?.Invoke(this, ex);
                return false;
            }
        }
        else
        {
            _event.Wait(_cancellation);
        }

        return IsUsable;
    }
}
