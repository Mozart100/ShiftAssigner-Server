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
}



public static class AdvanceCollectionExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> list)
    {
        if (list is null || list.FirstOrDefault() is  null )
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