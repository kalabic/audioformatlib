namespace AudioFormatLib.Buffers;

/// <summary> WIP </summary>
public class AudioStreamBuffer : CircularBufferStream
{
    private AFrameFormat _format;

    private int _minBufferSize = 0;

    public AudioStreamBuffer(AFrameFormat audioFormat, int bufferSeconds, CancellationToken cancellation) 
        : base((int)audioFormat.BufferSizeFromSeconds(bufferSeconds), cancellation)
    {
        _format = audioFormat;
    }

    public void SetWaitMinimumData(int miliseconds)
    {
        _minBufferSize = (int)_format.BufferSizeFromMiliseconds(miliseconds);
    }

    // TODO: Make async version
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_minBufferSize > 0)
        {
            int available = GetBytesAvailable(0);
            int minAsked = (_minBufferSize < count) ? _minBufferSize : count;
            if (available < minAsked)
            {
                if (!WaitDataAvailable(minAsked))
                {
                    return 0;
                }
            }
        }

        return base.Read(buffer, offset, count);
    }
}
