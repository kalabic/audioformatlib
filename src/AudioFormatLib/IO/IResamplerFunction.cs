namespace AudioFormatLib.IO;


public interface IResamplerFunction<IN, OUT> : IResamplerProperties, IDisposable
{
    long Process(bool lastPacket, IN input, long inputSize, OUT output, long outputSize);
}


public interface IResamplerFunction_BytePtr : IResamplerProperties, IDisposable
{
    unsafe long Process(bool lastPacket, byte* input, long inputSize, byte* output, long outputSize);
}
