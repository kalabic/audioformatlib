namespace AudioFormatLib.IO;


/// <summary> WIP </summary>
public interface IAudioBuffer : IDisposable
{
    int AllocatedSize { get; }

    int AvailableSpace { get; }

    bool IsClosed { get; }

    int StoredByteCount { get; }

    void ClearBuffer();

    void CloseBuffer();

    IAudioBufferInput GetBufferInput();

    IAudioBufferOutput GetBufferOutput();

    IAudioStreamInput GetStreamInput();

    IAudioStreamOutput GetStreamOutput();
}
