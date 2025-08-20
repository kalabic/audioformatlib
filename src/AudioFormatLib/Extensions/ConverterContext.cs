namespace AudioFormatLib.Extensions;


/// <summary>
/// 
/// It is very likely most of conversions will not require any additional context
/// for future Vector based conversions or they will be able to reuse the same one.
/// 
/// <para>WIP</para>
/// 
/// </summary>
public readonly struct ConverterContext
{
    public readonly struct Context<TYPE>
        where TYPE : unmanaged
    {
        public readonly int CTX;
    }

#pragma warning disable CS0169 // Variable is declared but never used
    private readonly Context<float> context;
#pragma warning restore CS0169 // Variable is declared but never used

    public bool GetContext(out Context<float>? cp3)
    {
        cp3 = null;
        return false;
    }
}
