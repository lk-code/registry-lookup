namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IInitializableRegistryHost
{
    Uri Host { get; }

    void Initialize(Uri hostUrl);
}