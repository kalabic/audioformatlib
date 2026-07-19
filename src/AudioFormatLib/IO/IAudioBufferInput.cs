using System;

namespace AudioFormatLib.IO;


public interface IAudioBufferInput
{
    APcmFormat Format { get; }

    void ClearBuffer();

    int Write(byte[] buffer, int offset, int count);

    int Write(short[] buffer, int offset, int count);
}
