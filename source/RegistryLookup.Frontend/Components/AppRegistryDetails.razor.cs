using dev.lkcode.RegistryLookup.Abstractions;
using Microsoft.AspNetCore.Components;

namespace dev.lkcode.RegistryLookup.Frontend.Components;

public partial class AppRegistryDetails : ComponentBase
{
    [Parameter]
    public required IRegistryItem RegistryItem { get; set; }
}