using Content.Shared.Actions;
using Robust.Shared.Serialization;

namespace Content.Shared._CM14.Xeno;

[Serializable, NetSerializable]
public sealed class XenoEvolutionBoundInterfaceState(
    float evolution,
    float modifer,
    HashSet<XenoEvolution> evolutions,
    bool enabled,
    Dictionary<int, int> limit,
    Dictionary<int, int> tiers)
    : BoundUserInterfaceState
{
    public readonly float Evolution = evolution;
    public readonly float EvolutionModifer = modifer;
    public readonly HashSet<_CM14.Xeno.XenoEvolution> Evolutions = evolutions;
    public readonly bool Enabled = enabled;
    public readonly Dictionary<int, int> Limit = limit;
    public readonly Dictionary<int, int> Tiers = tiers;
}

[Serializable, NetSerializable]
public sealed class EvolveMessage(XenoEvolution evolution) : BoundUserInterfaceMessage
{
    public readonly XenoEvolution Evolution = evolution;
}

public sealed partial class XenoEvolutionActionEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public enum XenoEvolutionUiKey
{
    Key
}
