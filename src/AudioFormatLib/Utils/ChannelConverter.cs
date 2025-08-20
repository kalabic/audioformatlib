using AudioFormatLib.Converters;
using AudioFormatLib.Extensions;

namespace AudioFormatLib.Utils;


public static unsafe class ChannelConverter
{
    private static readonly DelegateMap[][] _mappingArray = 
    [
        [PlanarConverter.Mapping, PlanarToInterleaved.Mapping],
        [InterleavedToPlanar.Mapping, InterleavedConverter.Mapping]
    ];

    private static DelegateMap GetDelegateMap(AChannelId source, AChannelId destination)
    {
        var mapping = ATools.ValidateChannelMapping(source, destination);
        if (mapping == ChannelMapping.UNSUPPORTED)
        {
            return DelegateMap.NONE;
        }

        var SMI = mapping.SourceLayout();
        var DMI = mapping.DestinationLayout();
        if (!SMI.IsValid() || !DMI.IsValid())
        {
            return DelegateMap.NONE;
        }

        return _mappingArray[(int)SMI][(int)DMI];
    }

    public static unsafe ConverterParams Get_Func<IN, OUT>(in AChannelId destination)
        where IN : unmanaged
        where OUT : unmanaged
    {
        var DMAP = GetDelegateMap(AChannelId.MonoTrack, destination);
        var FUNC = DMAP.GetDelegate<IN, OUT>();
        var CP = new ConverterParams(AChannelId.MonoTrack, destination)
        {
            Func = FUNC
        };
        return CP;
    }

    public static unsafe ConverterParams Get_Func<IN, OUT>(in AChannelId source, in AChannelId destination)
        where IN : unmanaged
        where OUT : unmanaged
    {
        var DMAP = GetDelegateMap(source, destination);
        var FUNC = DMAP.GetDelegate<IN, OUT>();
        var CP = new ConverterParams(source, destination)
        {
            Func = FUNC
        };
        return CP;
    }

    public static unsafe ConverterParams Get_Func(in AudioSpan source, in AudioSpan destination)
    {
        var DMAP = GetDelegateMap(AChannelId.EveryChannel, AChannelId.EveryChannel);
        var FUNC = DMAP.GetDelegate(source.SampleFormat, destination.SampleFormat);
        var CP = new ConverterParams(AChannelId.EveryChannel, AChannelId.EveryChannel)
        {
            Func = FUNC
        };
        return CP;
    }

    public static unsafe ConverterParams Get_Func(in AudioSpan source, 
                                                  in AudioSpan destination,
                                                  in AChannelId destinationChannel)
    {
        var DMAP = GetDelegateMap(AChannelId.MonoTrack, destinationChannel);
        var FUNC = DMAP.GetDelegate(source.SampleFormat, destination.SampleFormat);
        var CP = new ConverterParams(AChannelId.MonoTrack, destinationChannel)
        {
            Func = FUNC
        };
        return CP;
    }

    public static unsafe ConverterParams Get_Func(in AudioSpan source, in AudioSpan destination,
                                                  in AChannelId sourceChannel, in AChannelId destinationChannel)
    {
        var DMAP = GetDelegateMap(sourceChannel, destinationChannel);
        var FUNC = DMAP.GetDelegate(source.SampleFormat, destination.SampleFormat);
        var CP = new ConverterParams(sourceChannel, destinationChannel)
        {
            Func = FUNC
        };
        return CP;
    }
}
