using AudioFormatLib.IO;

namespace AudioFormatLib.Buffers;

/// <summary> WIP </summary>
public class NullBuffer : IAudioBuffer
{
    public int AllocatedSize => throw new NotImplementedException();

    public int AvailableSpace => throw new NotImplementedException();

    public bool IsClosed => throw new NotImplementedException();

    public int StoredByteCount => throw new NotImplementedException();

    public void ClearBuffer()
    {
        throw new NotImplementedException();
    }

    public void CloseBuffer()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public IAudioBufferInput GetBufferInput()
    {
        throw new NotImplementedException();
    }

    public IAudioBufferOutput GetBufferOutput()
    {
        throw new NotImplementedException();
    }

    public IAudioStreamInput GetStreamInput()
    {
        throw new NotImplementedException();
    }

    public IAudioStreamOutput GetStreamOutput()
    {
        throw new NotImplementedException();
    }
}
