using AudioFormatLib.IO;

namespace AudioFormatLib.Buffers;

/// <summary> WIP </summary>
public class NullBuffer : IAudioBuffer
{
    public int AllocatedSize => throw new NotImplementedException();

    public int AvailableSpace => throw new NotImplementedException();

    public bool IsClosed => throw new NotImplementedException();

    public int StoredByteCount => throw new NotImplementedException();

    public AFrameFormat Format => throw new NotImplementedException();

    public IAudioInputs Input => throw new NotImplementedException();

    public IAudioOutputs Output => throw new NotImplementedException();

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
}
