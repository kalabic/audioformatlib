using System.Diagnostics;

namespace AudioFormatLib;


/// <summary> WIP </summary>
public struct ABufferParams
{
    public const int DEFAULT_SIZE_MILISECONDS = 500;

    public bool WaitForCompleteRead { get; set; }

    public AFrameFormat Format { get; set; }

    public int BufferSize 
    { 
        get { return _bufferSize; } 

        set 
        { 
            Debug.Assert(value > 0);
            Debug.Assert((value % Format.FrameSampleSize) == 0);
            _bufferSize = value;
        }
    }

    private int _bufferSize;

    public ABufferParams()
    {
    }

    public ABufferParams(AFrameFormat format)
    {
        Format = format;
        BufferSize = (int)format.BufferSizeFromMiliseconds(DEFAULT_SIZE_MILISECONDS);
    }
}
