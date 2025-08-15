namespace AudioFormatLib.IO;


/// <summary>
/// 
/// Provides System.IO.Stream interface as buffer output, so applications can read data using it.
/// 
/// </summary>
public abstract class IAudioStreamOutput : Stream
{
    public abstract AFrameFormat Format { get; }

    //
    // System.IO.Stream
    //

    public override bool CanRead { get { return true; } }

    public override bool CanSeek { get { return false; } }

    public override bool CanWrite { get { return false; } }

    //
    // System.IO.Stream : Not implemented
    //

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush() => throw new NotImplementedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
}
