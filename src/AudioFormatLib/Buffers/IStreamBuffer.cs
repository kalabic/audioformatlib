namespace AudioFormatLib.Buffers;

/// <summary> WIP </summary>
public interface IStreamBuffer : IDisposable
{
    public bool CanRead { get; }

    public abstract bool IsClosed { get; }

    public abstract void Cancel();

    public abstract void ClearBuffer();

    public abstract int GetBytesAvailable();

    public abstract int GetBytesUnused();

    public abstract void SetBufferRequest(int value);

    public abstract int GetBufferRequest();

    public abstract Stream GetInputStream();

    public abstract Stream GetOutputStream();

    /// <summary>
    /// Read packet of data exactly the size of provided buffer and write it into other provided stream.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public abstract int MovePacket(IStreamBuffer other, byte[] buffer);
}
