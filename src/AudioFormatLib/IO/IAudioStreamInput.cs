namespace AudioFormatLib.IO;


/// <summary>
/// 
/// Provides System.IO.Stream interface as buffer input, so applications can write data using it.
/// 
/// </summary>
public abstract class IAudioStreamInput : Stream
{
    public abstract AFrameFormat Format { get; }

    //
    // System.IO.Stream
    //

    public override bool CanRead { get { return false; } }

    public override bool CanSeek { get { return false; } }

    public override bool CanWrite { get { return true; } }

    //
    // System.IO.Stream : Not implemented
    //

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush() => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();
}
