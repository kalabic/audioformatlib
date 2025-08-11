namespace AudioFormatLib.IO;

public interface IAudioBufferInput
{
    public int Write(short[] buffer, int offset, int count);
}
