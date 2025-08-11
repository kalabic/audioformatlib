using System.Diagnostics;

namespace AudioFormatLib.Utils;


/// <summary>
/// 
/// Parameters for conversion needs to contain information about channel layouts inside
/// source and destination frames, and identifiers of source and destination channels.
/// 
/// </summary>
public class ConverterParams
{
    /// <summary>
    /// Value used by Apple (Core Audio)1, ALSA2, MatLab2, sndlib2.
    /// <para>[1] Link: <a href="https://web.archive.org/web/20210210064026/http://blog.bjornroche.com/2009/12/int-float-int-its-jungle-out-there.html">Int->Float->Int: It's a jungle out there! (Web Archive)</a></para>
    /// <para>[2] Link: <a href="http://www.mega-nerd.com/libsndfile/FAQ.html#Q010">Q10: Reading a 16 bit PCM file as normalised floats...</a></para>
    /// </summary>
    public const float CONVERT_FACTOR_SHORT = 32768.0f;

    public readonly AChannelId SrcChannel;

    public readonly AChannelId DstChannel;

    public readonly ChannelConverter.Mapping MappingType;

    public ConverterParams(AChannelId source, AChannelId destination)
    {
        SrcChannel = source;
        DstChannel = destination;
        MappingType = AudioFrameTools.ValidateChannelMapping(source, destination);
        Debug.Assert(MappingType != ChannelConverter.Mapping.UNSUPPORTED);
    }
}
