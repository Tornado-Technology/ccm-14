using Content.Server.Actions;
using Content.Shared._CM14.Xeno;

namespace Content.Server._CM14.Xeno;

public sealed partial class XenoSystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;

    private void OnActionsStartup(Entity<XenoComponent> xeno, ref ComponentStartup args)
    {
        foreach (var action in xeno.Comp.Actions)
        {
            _actions.AddAction(xeno, action);
        }
    }
}
