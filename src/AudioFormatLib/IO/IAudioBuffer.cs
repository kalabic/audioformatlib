namespace AudioFormatLib.IO;


/// <summary> WIP </summary>
public interface IAudioBuffer : IDisposable
{
    int AllocatedSize { get; }

    int AvailableSpace { get; }

    APcmFormat Format { get; }

    bool IsClosed { get; }

    int StoredByteCount { get; }

    /// <summary> Total scalar sample values stored across all channels. </summary>
    int StoredSampleValueCount { get; }

    /// <summary> Temporal samples stored; equivalently, samples stored in each channel. </summary>
    int StoredSampleCount { get; }

    IAudioInputs Input { get; }

    IAudioOutputs Output { get; }

    void ClearBuffer();

    void CloseBuffer();
}
