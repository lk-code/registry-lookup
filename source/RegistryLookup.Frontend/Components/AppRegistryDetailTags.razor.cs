using dev.lkcode.RegistryLookup.Abstractions;
using Microsoft.AspNetCore.Components;

namespace dev.lkcode.RegistryLookup.Frontend.Components;

public partial class AppRegistryDetailTags : ComponentBase
{
    [Parameter]
    public required IRegistryItem RegistryItem { get; set; }

    [Parameter]
    public required ITaggable TaggableItem { get; set; }

    private List<string> _tagItems = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            IReadOnlyCollection<string> tagItems = await TaggableItem.GetTagsAsync(CancellationToken.None);
            
            await InvokeAsync(() =>
            {
                _tagItems = tagItems.ToList();

                StateHasChanged();
            });
        }
    }
}