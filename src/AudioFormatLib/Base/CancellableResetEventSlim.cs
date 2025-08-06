namespace AudioFormatLib.Base;

public class CancellableResetEventSlim : IDisposable
{
    public event EventHandler<Exception>? ExceptionOccured;

    public bool IsCancellationRequested { get {  return _cancellation.IsCancellationRequested; } }

    private CancellationToken _cancellation;

    private ManualResetEventSlim _cancellationEvent = new ManualResetEventSlim();

    private ManualResetEventSlim _event = new ManualResetEventSlim();

    private WaitHandle[] _waitHandles;

    public CancellableResetEventSlim(CancellationToken cancellation)
    {
        _cancellation = cancellation;
        _cancellation.Register( () => _cancellationEvent.Set() );
        _waitHandles = [_event.WaitHandle, _cancellationEvent.WaitHandle];
    }

    public void Dispose()
    {
        _cancellationEvent.Dispose();
        _event.Dispose();
    }

    public void Cancel()
    {
        _cancellationEvent.Set();
    }

    public bool Reset()
    {
        bool isActive = !_cancellation.IsCancellationRequested;
        if (isActive)
        {
            _event.Reset();
        }
        else
        {
            if (!_event.IsSet)
            {
                _event.Set();
            }
        }
        return isActive;
    }

    public bool Set()
    {
        bool isActive = !_cancellation.IsCancellationRequested;
        if (isActive)
        {
            _event.Set();
        }
        else
        {
            if (!_event.IsSet)
            {
                _event.Set();
            }
        }
        return isActive;
    }

    public bool Wait(bool noExceptions = true)
    {
        bool isActive = !_cancellation.IsCancellationRequested;
        int index = -1;
        if (isActive)
        {
            if (noExceptions)
            {
                try
                {
                    index = WaitHandle.WaitAny(_waitHandles);
                }
                catch (OperationCanceledException ex)
                {
                    ExceptionOccured?.Invoke(this, ex);
                }
                catch (ObjectDisposedException ex)
                {
                    ExceptionOccured?.Invoke(this, ex);
                }
                catch (InvalidOperationException ex)
                {
                    ExceptionOccured?.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    ExceptionOccured?.Invoke(this, ex);
                }
            }
            else
            {
                index = WaitHandle.WaitAny(_waitHandles);
            }

            isActive = !_cancellation.IsCancellationRequested;
        }

        if (!isActive)
        {
            if (!_event.IsSet)
            {
                _event.Set();
            }
        }

        return isActive && (index == 0);
    }
}
