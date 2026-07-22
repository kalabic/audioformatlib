namespace AudioFormatLib.IO;


public interface IAudioOutputs
{
    APcmFormat Format { get; }

    IAudioBufferOutput Buffer { get; }

    /// <summary>Signed 16-bit PCM value access, or <c>null</c> for an incompatible format.</summary>
    IPcm16ValueOutput? Pcm16Values { get; }

    /// <summary>
    /// Complete signed 16-bit PCM frame access, or <c>null</c> for an incompatible or
    /// multi-channel planar format.
    /// </summary>
    IPcm16FrameOutput? Pcm16Frames { get; }

    IAudioStreamOutput Stream { get; }
}
