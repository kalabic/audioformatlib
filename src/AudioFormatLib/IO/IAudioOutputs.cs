namespace AudioFormatLib.IO;


public interface IAudioOutputs
{
    AFrameFormat Format { get; }

    IAudioBufferOutput Buffer { get; }

    IAudioStreamOutput Stream { get; }
}
