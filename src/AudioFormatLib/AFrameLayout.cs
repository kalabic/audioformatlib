namespace AudioFormatLib;

public enum AFrameLayout : int
{
    NONE = -1,
    PLANAR = 0,
    INTERLEAVED = 1,
}


public static class AFrameLayoutMethods
{
    public static bool IsValid(this AFrameLayout frame)
    {
        return (0 <= (int)frame) && ((int)frame <= 1);
    }
}
