using AudioFormatLib.IO;

namespace AudioFormatLib.Buffers;


internal class SharedAudioStreamInput : IAudioStreamInput
{
    public override AFrameFormat Format => throw new NotImplementedException();


    private readonly AFrameFormat _format;

    /// <summary> Externally managed buffer. </summary>
    private readonly IUnsafeBuffer _buffer;

    public SharedAudioStreamInput(AFrameFormat format, IUnsafeBuffer sharedBuffer)
    {
        _format = format;
        _buffer = sharedBuffer;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _buffer.Write(buffer, offset, count);
    }
}
