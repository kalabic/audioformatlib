using AudioFormatLib.Utils;

namespace AudioFormatLib.Converters;

public unsafe readonly struct DelegateArray
{
    public readonly delegate*<in ConverterParams, in AudioSpan, in AudioSpan, void>[] Delegates;

    public DelegateArray(delegate*<in ConverterParams, in AudioSpan, in AudioSpan, void> delegate0,
                         delegate*<in ConverterParams, in AudioSpan, in AudioSpan, void> delegate1)
    {
        Delegates = [delegate0, delegate1];
    }
}

public readonly struct DelegateMap
{
    public static readonly DelegateMap NONE = new DelegateMap();

    public bool IsEmpty { get { return Map.Length == 0; } }

    public readonly DelegateArray[] Map;


    public DelegateMap()
    {
        Map = Array.Empty<DelegateArray>();
    }

    public DelegateMap(DelegateArray row0, DelegateArray row1)
    {
        Map = [row0, row1];
    }

    public unsafe delegate*<in ConverterParams, in AudioSpan, in AudioSpan, void> GetDelegate(int STI, int DTI)
    {
        if (Map.Length == 0 || (STI < 0) || (1 < STI) || (DTI < 0) || (1 < DTI))
        {
            return &ConverterParams.NOOP;
        }

        return Map[STI].Delegates[DTI];
    }

    public unsafe delegate*<in ConverterParams, in AudioSpan, in AudioSpan, void> GetDelegate(ASampleFormat source, ASampleFormat destination)
    {
        var STI = source.ConverterIndex();
        var DTI = destination.ConverterIndex();
        return GetDelegate(STI, DTI);
    }

    public unsafe delegate*<in ConverterParams, in AudioSpan, in AudioSpan, void> GetDelegate<IN, OUT>()
        where IN : unmanaged
        where OUT : unmanaged
    {
        var STI = ASampleFormat.NONE.DefaultForType<IN>().ConverterIndex();
        var DTI = ASampleFormat.NONE.DefaultForType<OUT>().ConverterIndex();
        return GetDelegate(STI, DTI);
    }
}
