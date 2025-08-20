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
    public static AFrameLayout SourceLayout(this ChannelMapping mapping)
    {
        switch (mapping)
        {
            case ChannelMapping.PLANAR:
                return AFrameLayout.PLANAR;

            case ChannelMapping.PLANAR_TO_INTERLEAVED:
                return AFrameLayout.PLANAR;

            case ChannelMapping.INTERLEAVED_TO_PLANAR:
                return AFrameLayout.INTERLEAVED;

            case ChannelMapping.INTERLEAVED:
                return AFrameLayout.INTERLEAVED;
        }

        return AFrameLayout.NONE;
    }

    public static AFrameLayout DestinationLayout(this ChannelMapping mapping)
    {
        switch (mapping)
        {
            case ChannelMapping.PLANAR:
                return AFrameLayout.PLANAR;

            case ChannelMapping.PLANAR_TO_INTERLEAVED:
                return AFrameLayout.INTERLEAVED;

            case ChannelMapping.INTERLEAVED_TO_PLANAR:
                return AFrameLayout.PLANAR;

            case ChannelMapping.INTERLEAVED:
                return AFrameLayout.INTERLEAVED;
        }

        return AFrameLayout.NONE;
    }
}
