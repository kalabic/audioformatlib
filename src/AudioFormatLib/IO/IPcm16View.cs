namespace AudioFormatLib.IO;


/// <summary>Describes a view of signed 16-bit PCM stored by an audio buffer.</summary>
public interface IPcm16View
{
    APcmFormat Format { get; }

    bool IsOpen { get; }
}
