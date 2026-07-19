namespace AudioFormatLib.IO;


public interface IAudioInputs
{
    APcmFormat Format { get; }

    IAudioBufferInput Buffer { get; }

    IAudioStreamInput Stream { get; }
}
