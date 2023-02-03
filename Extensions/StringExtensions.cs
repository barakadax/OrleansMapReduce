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

    public static bool IsNullOrEmpty(this Dictionary<object, object> obj)
    {
        return obj == null || obj.Count == 0;
    }

    public static bool NotNullNorEmpty(this Dictionary<object, object> obj)
    {
        return obj != null && obj.Count != 0;
    }

    public static bool IsNullOrEmpty(this Dictionary<ulong, ulong> obj)
    {
        return obj == null || obj.Count == 0;
    }

    public static bool NotNullNorEmpty(this Dictionary<ulong, ulong> obj)
    {
        return obj != null && obj.Count != 0;
    }
}
