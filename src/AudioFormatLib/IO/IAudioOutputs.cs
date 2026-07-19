namespace AudioFormatLib.IO;


public interface IAudioOutputs
{
    APcmFormat Format { get; }

    IAudioBufferOutput Buffer { get; }

    IAudioStreamOutput Stream { get; }
}
