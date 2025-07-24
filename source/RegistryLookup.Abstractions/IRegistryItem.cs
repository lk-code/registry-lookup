namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IRegistryItem
{
    IRegistryHost ParentHost { get; }
    string Name { get; }
}