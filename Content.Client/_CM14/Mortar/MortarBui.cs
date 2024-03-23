using System.Numerics;
using Content.Shared._CM14.Mortar;
using JetBrains.Annotations;

namespace Content.Client._CM14.Mortar;

[UsedImplicitly]
public sealed class MortarBui : BoundUserInterface
{
    [ViewVariables]
    private MortarMenu? _menu;

    public MortarBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = new MortarMenu(this);
        _menu.OpenCentered();
        _menu.OnClose += Close;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is not MortarBoundUserInterfaceState castState)
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

    public void Save(int index, Vector2 position)
    {
        SendMessage(new MortarSaveMessage(index, position));
    }

    public void Launch(int index)
    {
        SendMessage(new MortarLaunchMessage(index));
    }
}
