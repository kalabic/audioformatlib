namespace AudioFormatLib.IO;


/// <summary> WIP </summary>
public interface IAudioBuffer : IDisposable
{
    int AllocatedSize { get; }

    int AvailableSpace { get; }

    AFrameFormat Format { get; }

    bool IsClosed { get; }

    int StoredByteCount { get; }

    IAudioInputs Input { get; }

    IAudioOutputs Output { get; }

    void ClearBuffer();

    void CloseBuffer();
}
