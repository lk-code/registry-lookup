namespace dev.lkcode.RegistryLookup.Core.Extensions;

public static class UriExtensions
{
    /// <summary>
    /// Removes the specified character from the end of the URI string, if present, and returns a new Uri.
    /// </summary>
    /// <param name="uri">The source URI.</param>
    /// <param name="trimChar">The character to trim from the end.</param>
    /// <returns>A new Uri without the trailing character, if it was present.</returns>
    public static Uri TrimEnd(this Uri uri, char trimChar)
    {
        var uriString = uri.ToString();
        if (uriString.EndsWith(trimChar))
            uriString = uriString[..^1];
        return new Uri(uriString);
    }
}