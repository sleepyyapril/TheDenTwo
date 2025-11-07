using Content.Shared._DEN.ResourceOverview.Components;
using Robust.Client.UserInterface;


namespace Content.Client._DEN.ResourceOverview.UI;


public sealed class ResourceOverviewConsoleBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private ResourceOverviewConsoleWindow? _menu;

    public ResourceOverviewConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) { }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<ResourceOverviewConsoleWindow>();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not ResourceOverviewBoundInterfaceState castState)
            return;

        _menu?.UpdateState(castState);
    }
}
