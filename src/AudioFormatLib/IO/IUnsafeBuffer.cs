namespace AudioFormatLib.IO;

internal interface IUnsafeBuffer
{
    int Count { get; }

    bool IsOpen { get; }

    int MaxLength { get; }

    void Close();

    int Read(byte[] data, int offset, int count);

    unsafe int Read(byte* dataPtr, int offset, int count);

    void Reset();

    int Write(byte[] data, int offset, int count);

    unsafe int Write(byte* data, int offset, int count);
}
