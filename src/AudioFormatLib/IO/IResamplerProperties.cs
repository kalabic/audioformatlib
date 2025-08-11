namespace AudioFormatLib.IO;

public interface IResamplerProperties
{
    AResamplerParams Params { get; }

    public long InPacketCount { get; }

    public long InBytesProcessed { get; }

    public long OutBytesGenerated { get; }
}
