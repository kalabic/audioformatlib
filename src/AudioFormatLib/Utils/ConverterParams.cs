using AudioFormatLib.Extensions;
using System.Diagnostics;

namespace AudioFormatLib.Utils;


/// <summary>
/// 
/// Parameters for conversion needs to contain information about channel layouts inside
/// source and destination frames, and identifiers of source and destination channels.
/// 
/// </summary>
public unsafe struct ConverterParams
{
    public static readonly ConverterParams NONE = new ConverterParams();

    public static void NOOP(in ConverterParams context,
                            in AudioSpan input,
                            in AudioSpan output)
    { /* A safe no-op when no converters were found. */ }

    /// <summary>
    /// Value used by Apple (Core Audio)1, ALSA2, MatLab2, sndlib2.
    /// <para>[1] Link: <a href="https://web.archive.org/web/20210210064026/http://blog.bjornroche.com/2009/12/int-float-int-its-jungle-out-there.html">Int->Float->Int: It's a jungle out there! (Web Archive)</a></para>
    /// <para>[2] Link: <a href="http://www.mega-nerd.com/libsndfile/FAQ.html#Q010">Q10: Reading a 16 bit PCM file as normalised floats...</a></para>
    /// </summary>
    public const float CONVERT_FACTOR_SHORT = 32768.0f;

    public bool Valid { get {  return MappingType != ChannelMapping.UNSUPPORTED; } }

    public readonly AChannelId SrcChannel;

    public readonly AChannelId DstChannel;

    public readonly ChannelMapping MappingType;

    public delegate*<in ConverterParams, in AudioSpan, in AudioSpan, void> Func = null;

    public ConverterParams()
    {
        SrcChannel = AChannelId.EveryChannel;
        DstChannel = AChannelId.EveryChannel;
        MappingType = ChannelMapping.UNSUPPORTED;
        Func = &NOOP;
    }

    /// <summary> Used when converter needs to convert each and every channel. </summary>
    public ConverterParams(AChannelId source)
    {
        Debug.Assert(source.AllChannels);
        this.SrcChannel = source;
        this.DstChannel = source;
        MappingType = ATools.ValidateChannelMapping(source, source);
        Debug.Assert(MappingType != ChannelMapping.UNSUPPORTED);
    }

    public ConverterParams(AChannelId source, AChannelId destination)
    {
        this.SrcChannel = source;
        this.DstChannel = destination;
        MappingType = ATools.ValidateChannelMapping(source, destination);
        Debug.Assert(MappingType != ChannelMapping.UNSUPPORTED);
    }
}
