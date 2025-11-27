public static class StringExtensions
{
    public static bool IsEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsNotEmpty(this string str)
    {
        return !IsEmpty(str);
    }

    public static bool IsEqual(this string source, string target)
    {

        if (source.IsEmpty() && target.IsEmpty())
        {
            return true;
        }

        if (source.IsNotEmpty() && target.IsNotEmpty())
        {
            return source.Equals(target, StringComparison.InvariantCultureIgnoreCase);
        }

        return false;
    }

}



public static class AdvanceCollectionExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> list)
    {
        if (list is null || list.FirstOrDefault() is null)
        {
            return true;
        }

        return false;
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> list)
    {
        return !IsEmpty(list);
    }
}