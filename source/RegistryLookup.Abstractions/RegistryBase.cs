namespace dev.lkcode.RegistryLookup.Abstractions;

public abstract class RegistryBase : IInitializableRegistryHost
{
    public Uri Host { get; private set; }

    public void Initialize(Uri hostUrl)
    {
        Host = hostUrl;
    }
}