namespace AudioFormatLib.Buffers;

/// <summary> WIP </summary>
public class NullBuffer : Stream, IStreamBuffer
{
    public bool IsClosed { get { return false; } }

    public override bool CanRead => throw new NotImplementedException();

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanWrite => throw new NotImplementedException();

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Cancel()
    {
        throw new NotImplementedException();
    }

    public void ClearBuffer()
    {
        throw new NotImplementedException();
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public int GetBytesAvailable()
    {
        throw new NotImplementedException();
    }

    public int GetBytesUnused()
    {
        throw new NotImplementedException();
    }

    public int GetBufferRequest()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public void SetBufferRequest(int value)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public int MovePacket(IStreamBuffer other, byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public Stream GetInputStream()
    {
        throw new NotImplementedException();
    }

    public Stream GetOutputStream()
    {
        throw new NotImplementedException();
    }
}
