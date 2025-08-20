using AudioFormatLib.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AudioFormatLib.Extensions;

public static unsafe class FunctionBridge
{
    private static Func<AudioSpan, AudioSpan, ConverterParams?, long>? _currentFunc;

    // CS8977: Cannot use 'ref', 'in', or 'out' in the signature of a method attributed with 'UnmanagedCallersOnly'
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static long BridgeFunction(AudioSpan input, AudioSpan output, ConverterParams? param)
    {
        if (_currentFunc == null)
            throw new InvalidOperationException("No function registered");

        return _currentFunc(input, output, param);
    }

    public static delegate* unmanaged[Cdecl]<AudioSpan, AudioSpan, ConverterParams?, long> GetFunctionPointer(Func<AudioSpan, AudioSpan, ConverterParams?, long> func)
    {
        _currentFunc = func;
        return &BridgeFunction;
    }
}
