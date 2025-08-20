using AudioFormatLib.Utils;
using System.Runtime.InteropServices;

namespace AudioFormatLib.Extensions;

public unsafe class SafeFunctionPointer : IDisposable
{
    private readonly GCHandle _handle;
    public readonly delegate*<AudioSpan, AudioSpan, ConverterParams?, long> Pointer;

    public SafeFunctionPointer(Func<AudioSpan, AudioSpan, ConverterParams?, long> managedFunc)
    {
        _handle = GCHandle.Alloc(managedFunc);
        IntPtr ptr = Marshal.GetFunctionPointerForDelegate(managedFunc);
        Pointer = (delegate*<AudioSpan, AudioSpan, ConverterParams?, long>)ptr.ToPointer();
    }

    public void Dispose()
    {
        if (_handle.IsAllocated)
            _handle.Free();
        GC.SuppressFinalize(this);
    }

    ~SafeFunctionPointer() => Dispose();
}
