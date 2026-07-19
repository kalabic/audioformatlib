namespace AudioFormatLib.Extensions;

public enum ChannelMapping : int
{
    UNSUPPORTED = -1,
    PLANAR = 0,
    PLANAR_TO_INTERLEAVED = 1,
    INTERLEAVED_TO_PLANAR = 2,
    INTERLEAVED = 3,
}

public static class ChannelMappingMethods
{
    public static APcmLayout SourceLayout(this ChannelMapping mapping)
    {
        switch (mapping)
        {
            case ChannelMapping.PLANAR:
                return APcmLayout.PLANAR;

            case ChannelMapping.PLANAR_TO_INTERLEAVED:
                return APcmLayout.PLANAR;

            case ChannelMapping.INTERLEAVED_TO_PLANAR:
                return APcmLayout.INTERLEAVED;

            case ChannelMapping.INTERLEAVED:
                return APcmLayout.INTERLEAVED;
        }

        return APcmLayout.NONE;
    }

    public static APcmLayout DestinationLayout(this ChannelMapping mapping)
    {
        switch (mapping)
        {
            case ChannelMapping.PLANAR:
                return APcmLayout.PLANAR;

            case ChannelMapping.PLANAR_TO_INTERLEAVED:
                return APcmLayout.INTERLEAVED;

            case ChannelMapping.INTERLEAVED_TO_PLANAR:
                return APcmLayout.PLANAR;

            case ChannelMapping.INTERLEAVED:
                return APcmLayout.INTERLEAVED;
        }

        return APcmLayout.NONE;
    }
}
