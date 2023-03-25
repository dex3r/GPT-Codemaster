namespace AiProgrammer.Extensions;

public static class StringExtensions
{
    public static string? SubstringSafe(this string? source, int startIndex, int length)
    {
        if (source == null)
        {
            return null;
        }

        if (source.Length == 0)
        {
            return source;
        }

        if (startIndex >= source.Length)
        {
            return string.Empty;
        }

        length = Math.Min(length, source.Length - startIndex);
        
        return source.Substring(startIndex, length);
    }
}
