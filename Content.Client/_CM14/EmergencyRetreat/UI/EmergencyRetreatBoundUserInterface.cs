using Content.Shared._CM14.EmergencyRetreat;

namespace Content.Client._CM14.EmergencyRetreat.UI;

public sealed class EmergencyRetreatBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private EmergencyRetreatMenu? _menu;

    public EmergencyRetreatBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = new EmergencyRetreatMenu(this);
        _menu.OpenCentered();
        _menu.OnClose += Close;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is not EmergencyRetreatBoundUserInterfaceState castState)
            return;

        _menu?.UpdateState(castState);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        _menu?.Dispose();
    }

    public void SendRunFtl()
    {
        SendMessage(new EmergencyRetreatRunFtlMessage());
    }
}
