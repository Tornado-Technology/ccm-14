using System.Numerics;
using Content.Shared._CM14.Mortar;
using Content.Shared.Construction.Components;
using Robust.Server.GameObjects;

namespace Content.Server._CM14.Mortar;

public sealed partial class MortarSystem
{
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    private void InitializeUI()
    {
        SubscribeLocalEvent<MortarComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<MortarComponent, MortarSaveMessage>(OnSave);
        SubscribeLocalEvent<MortarComponent, MortarLaunchMessage>(OnLaunch);
    }

    private void OnStartup(Entity<MortarComponent> mortar, ref ComponentStartup args)
    {
        for (var i = 0; i < mortar.Comp.MaxSavedPositions; i++)
        {
            mortar.Comp.SavedPositions.Add(Vector2.Zero);
        }

        UpdateUI(mortar);
    }

    private void OnSave(Entity<MortarComponent> mortar, ref MortarSaveMessage args)
    {
        if (mortar.Comp.SavedPositions.Count < args.Index)
            return;

        mortar.Comp.SavedPositions[args.Index] = args.Position;
        UpdateUI(mortar);
    }

    private void OnLaunch(Entity<MortarComponent> mortar, ref MortarLaunchMessage args)
    {
        var user = args.Session.AttachedEntity;
        if (!Exists(user))
            return;

        if (!Transform(mortar).Anchored)
        {
            TryLaunch(mortar, _transform.GetMapCoordinates(mortar), user);
            return;
        }

        if (mortar.Comp.SavedPositions.Count < args.Index)
            return;

        TryLaunch(mortar, mortar.Comp.SavedPositions[args.Index], user);
        UpdateUI(mortar);
    }

    private void UpdateUI(Entity<MortarComponent> mortar)
    {
        _userInterface.TrySetUiState(mortar, MortarUiKey.Key, new MortarBoundUserInterfaceState(mortar.Comp.SavedPositions));
    }
}
