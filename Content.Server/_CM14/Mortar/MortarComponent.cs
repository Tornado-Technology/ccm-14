using System.Numerics;
using Robust.Shared.Utility;

namespace Content.Server._CM14.Mortar;

[RegisterComponent]
public sealed partial class MortarComponent : Component
{
    [DataField]
    public string ShellContainerId = "shell";

    [DataField]
    public int MaxSavedPositions = 3;

    [DataField]
    public List<Vector2> SavedPositions = new();

    [DataField]
    public ResPath LaunchSound = new("/Audio/Weapons/Guns/Gunshots/grenade_launcher.ogg");
}
