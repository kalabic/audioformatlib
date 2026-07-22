namespace AudioFormatLib.IO;


public interface IAudioInputs
{
    APcmFormat Format { get; }

    IAudioBufferInput Buffer { get; }

    /// <summary>Signed 16-bit PCM value access, or <c>null</c> for an incompatible format.</summary>
    IPcm16ValueInput? Pcm16Values { get; }

    /// <summary>
    /// Complete signed 16-bit PCM frame access, or <c>null</c> for an incompatible or
    /// multi-channel planar format.
    /// </summary>
    IPcm16FrameInput? Pcm16Frames { get; }

    IAudioStreamInput Stream { get; }
}
