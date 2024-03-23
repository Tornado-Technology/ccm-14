using System.Numerics;
using Robust.Shared.Serialization;

namespace Content.Shared._CM14.Mortar;

[Serializable, NetSerializable]
public sealed class MortarBoundUserInterfaceState : BoundUserInterfaceState
{
    public readonly IReadOnlyList<Vector2> SavedPoints;

    public MortarBoundUserInterfaceState(IReadOnlyList<Vector2> savedPoints)
    {
        SavedPoints = savedPoints;
    }
}

[Serializable, NetSerializable]
public sealed class MortarLaunchMessage : BoundUserInterfaceMessage
{
    public readonly int Index;

    public MortarLaunchMessage(int index)
    {
        Index = index;
    }
}

[Serializable, NetSerializable]
public sealed class MortarSaveMessage : BoundUserInterfaceMessage
{
    public readonly int Index;
    public readonly Vector2 Position;

    public MortarSaveMessage(int index, Vector2 position)
    {
        Index = index;
        Position = position;
    }
}

[Serializable, NetSerializable]
public enum MortarUiKey
{
    Key,
}
