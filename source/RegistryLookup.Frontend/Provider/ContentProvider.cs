using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Abstractions.Exceptions;

namespace dev.lkcode.RegistryLookup.Frontend.Provider;

public class ContentProvider(HttpClient httpClient) : IContentProvider
{
    public async Task<string> GetContentAsync(string file)
    {
        try
        {
            string content = await httpClient.GetStringAsync(file);
            return content;
        }
        catch (HttpRequestException err) when (err.Message.Contains("404") && err.Message.Contains("Not Found"))
        {
            throw new HttpNotFoundException(file, "response is not found.", err);
        }
    }
}