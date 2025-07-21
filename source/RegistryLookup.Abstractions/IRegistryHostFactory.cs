namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IRegistryHostFactory
{
    IRegistryHost Create(Uri hostUrl);
}