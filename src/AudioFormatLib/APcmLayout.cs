namespace AudioFormatLib;

public enum APcmLayout : int
{
    NONE = -1,
    PLANAR = 0,
    INTERLEAVED = 1,
}


public static class APcmLayoutMethods
{
    public static bool IsValid(this APcmLayout layout)
    {
        return (0 <= (int)layout) && ((int)layout <= 1);
    }
}
