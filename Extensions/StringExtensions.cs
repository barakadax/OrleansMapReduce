using System.Collections;

namespace Extensions; 

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string str)
    {
        return str == null || str.Length == 0;
    }

    public static bool NotNullNorEmpty(this string str)
    {
        return str != null && str.Length != 0;
    }

    public static bool IsNullOrEmpty(this Dictionary<object, object> str)
    {
        return str == null || str.Count == 0;
    }

    public static bool NotNullNorEmpty(this Dictionary<object, object> str)
    {
        return str != null && str.Count != 0;
    }

    public static bool IsNullOrEmpty(this Dictionary<ulong, ulong> str)
    {
        return str == null || str.Count == 0;
    }

    public static bool NotNullNorEmpty(this Dictionary<ulong, ulong> str)
    {
        return str != null && str.Count != 0;
    }
}
