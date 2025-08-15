namespace AudioFormatLib.IO;


public interface IAudioInputs
{
    AFrameFormat Format { get; }

    IAudioBufferInput Buffer { get; }

    IAudioStreamInput Stream { get; }
}
