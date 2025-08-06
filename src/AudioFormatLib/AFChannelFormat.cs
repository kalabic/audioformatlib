namespace AudioFormatLib;


/// <summary>
/// Work in progress.
/// </summary>
public readonly struct AFChannelFormat
{
    public int Count { get { return _count; } }

    private readonly int _count;

    public AFChannelFormat(int count)
    {
        _count = count;
    }
}
