using Content.Shared._CM14.Xeno;

namespace Content.Client._CM14.Xeno.UI;

public sealed partial class XenoEvolutionBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private XenoEvolutionMenu? _menu;

    public XenoEvolutionBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = new XenoEvolutionMenu(this);
        _menu.OpenCentered();
        _menu.OnClose += Close;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
            return;

        _menu?.Dispose();
    }


    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is not XenoEvolutionBoundInterfaceState cState)
            return;

        _menu?.UpdateState(cState);
    }

    public void Evolve(XenoEvolution evolution)
    {
        SendMessage(new EvolveMessage(evolution));
    }
}
