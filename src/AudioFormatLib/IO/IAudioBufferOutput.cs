namespace AudioFormatLib.IO;

public interface IAudioBufferOutput
{
    public int Read(short[] buffer, int offset, int count);
}
