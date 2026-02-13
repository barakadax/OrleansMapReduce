namespace Extensions;

public static class ExtensionsMethods
{
    public static bool IsNullOrEmpty(this string str)
    {
        return str is null || str.Length == 0;
    }

    public static bool NotNullNorEmpty(this string str)
    {
        return str is not null && str.Length != 0;
    }

    public static bool IsNullOrEmpty(this Dictionary<ulong, ulong> obj)
    {
        return obj is null || obj.Count == 0;
    }

    public static bool NotNullNorEmpty(this Dictionary<ulong, ulong> obj)
    {
        return obj is not null && obj.Count != 0;
    }
}
