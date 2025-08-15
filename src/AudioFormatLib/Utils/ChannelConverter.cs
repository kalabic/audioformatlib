using AudioFormatLib.Converters;

namespace AudioFormatLib.Utils;


public static class ChannelConverter
{
    public enum Mapping : int
    {
        UNSUPPORTED = 0,
        PLANAR = 1,
        INTERLEAVED_TO_PLANAR = 2,
        PLANAR_TO_INTERLEAVED = 3,
        INTERLEAVED = 4,
    }

    /// <summary>
    /// 
    /// Return appropriate function for sample conversion from source into destination,
    /// converting a single channel from audio frame, all while taking into consideration
    /// source and destination channel layouts.
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public static unsafe (AConversion_Float_To_ShortPtr Func, ConverterParams Params)? Get_Float_To_ShortPtr_Func(AChannelId source, AChannelId destination)
    {
        var parameters = new ConverterParams(source, destination);
        switch (parameters.MappingType)
        {
            case Mapping.PLANAR:
                return (PlanarConverter.Float_To_ShortPtr, parameters);

            case Mapping.PLANAR_TO_INTERLEAVED:
                return (PlanarToInterleaved.Float_To_ShortPtr, parameters);

            case Mapping.INTERLEAVED_TO_PLANAR:
                return (InterleavedToPlanar.Float_To_ShortPtr, parameters);

            case Mapping.INTERLEAVED:
                return (InterleavedConverter.Float_To_ShortPtr, parameters);
        }

        return null;
    }


    /// <summary>
    /// 
    /// Return appropriate function for sample conversion from source into destination,
    /// converting a single channel from audio frame, all while taking into consideration
    /// source and destination channel layouts.
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public static unsafe (AConversion_ShortPtr_To_Float Func, ConverterParams Params)? Get_ShortPtr_To_Float_Func(AChannelId source, AChannelId destination)
    {
        var parameters = new ConverterParams(source, destination);
        switch (parameters.MappingType)
        {
            case Mapping.PLANAR:
                return (PlanarConverter.ShortPtr_To_Float, parameters);

            case Mapping.PLANAR_TO_INTERLEAVED:
                return (PlanarToInterleaved.ShortPtr_To_Float, parameters);

            case Mapping.INTERLEAVED_TO_PLANAR:
                return (InterleavedToPlanar.ShortPtr_To_Float, parameters);

            case Mapping.INTERLEAVED:
                return (InterleavedConverter.ShortPtr_To_Float, parameters);
        }

        return null;
    }
}
