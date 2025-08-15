namespace AudioFormatLib.Utils;


public unsafe partial class AudioFrameResampler : AudioFrameResampler.IResampleFunction
{
    /// <summary>
    /// 
    /// Extensions here allow for (hopefully) more convenient way of writing code, for example:
    /// <code>
    /// long samplesGenerated = resampler
    ///                .AttachBuffers(packet.Data, packet.Size, buffer.Data, buffer.BytesAllocated)
    ///                .Resample(packet.LastPacket);
    /// </code>
    /// </summary>
    public interface IResampleFunction
    {
        long Resample(bool lastPacket);
    }

    public unsafe IResampleFunction AttachBuffers<IN, OUT>(IN[] input, long inputLength, OUT[] output, long outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        _coupler.AttachBuffers(input, inputLength, output, outputLength);
        return this;
    }

    public unsafe IResampleFunction AttachBuffers<IN, OUT>(IN* input, long inputLength, OUT[] output, long outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        _coupler.AttachBuffers(input, inputLength, output, outputLength);
        return this;
    }

    public unsafe IResampleFunction AttachBuffers<IN, OUT>(IN[] input, long inputLength, OUT* output, long outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        _coupler.AttachBuffers(input, inputLength, output, outputLength);
        return this;
    }

    public unsafe IResampleFunction AttachBuffers<IN, OUT>(IN* input, long inputLength, OUT* output, long outputLength)
         where IN : unmanaged
         where OUT : unmanaged
    {
        _coupler.AttachBuffers(input, inputLength, output, outputLength);
        return this;
    }
}
