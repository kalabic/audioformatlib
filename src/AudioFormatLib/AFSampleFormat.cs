namespace AudioFormatLib;


/// <summary>
/// Intention is to keep these the same/compatible as in AVSampleFormat found in FFmpeg library.
/// </summary>
public enum AFFormatID : int
{
    FMT_NONE = -1,

    /// <summary> Unsigned 8 bits </summary>
    FMT_U8 = 0,

    /// <summary> Signed 16 bits </summary>
    FMT_S16 = 1,

    /// <summary> Signed 32 bits </summary>
    FMT_S32 = 2,

    /// <summary> Float </summary>
    FMT_FLOAT = 3,

    /// <summary> Double </summary>
    FMT_DOUBLE = 4,

    /// <summary> Unsigned 8 bits, planar </summary>
    FMT_U8_P = 5,

    /// <summary> Signed 16 bits, planar </summary>
    FMT_S16_P = 6,

    /// <summary> Signed 32 bits, planar </summary>
    FMT_S32_P = 7,

    /// <summary> Float, planar </summary>
    FMT_FLOAT_P = 8,

    /// <summary> Double, planar </summary>
    FMT_DOUBLE_P = 9,

    /// <summary> Signed 64 bits </summary>
    FMT_S64 = 10,

    /// <summary> Signed 64 bits, planar </summary>
    FMT_S64_P = 11,
}


public struct AFSampleFormat
{
    public static int GetBitCount(AFFormatID id)
    {
        switch(id)
        {
            case AFFormatID.FMT_NONE:     // not initialized
                return 0;

            case AFFormatID.FMT_U8:       // unsigned 8 bits
            case AFFormatID.FMT_U8_P:     // unsigned 8 bits, planar
                return 8;

            case AFFormatID.FMT_S16:      // signed 16 bits
            case AFFormatID.FMT_S16_P:    // signed 16 bits, planar
                return 16;

            case AFFormatID.FMT_FLOAT:    // float
            case AFFormatID.FMT_FLOAT_P:  // float, planar
            case AFFormatID.FMT_S32:      // signed 32 bits
            case AFFormatID.FMT_S32_P:    // signed 32 bits, planar
                return 32;

            case AFFormatID.FMT_DOUBLE:   // double
            case AFFormatID.FMT_DOUBLE_P: // double, planar
            case AFFormatID.FMT_S64:      // signed 64 bits
            case AFFormatID.FMT_S64_P:    // signed 64 bits, planar
                return 64;

            default:
                throw new ArgumentException($"Invalid AFFormatID: {id}");
        }
    }

    public AFFormatID Id { get { return _id; } }

    public int Bits { get { return _bits; } }

    public int Bytes { get { return _bits / 8; } }

    AFFormatID _id;

    int _bits;

    public AFSampleFormat()
    {
        _id = AFFormatID.FMT_S16;
        _bits = 16;
    }

    public AFSampleFormat(AFFormatID id)
    {
        _id = id;
        _bits = GetBitCount(id);
    }
}
