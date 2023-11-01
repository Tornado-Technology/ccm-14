using Content.Shared.Actions;
using Robust.Shared.Serialization;

namespace Content.Shared.Xeno;

[Serializable, NetSerializable]
public sealed class XenoEvolutionBoundInterfaceState : BoundUserInterfaceState
{
    public readonly float Evolution;
    public readonly float EvolutionModifer;
    public readonly HashSet<XenoEvolution> Evolutions;
    public readonly bool Enabled;
    public readonly Dictionary<int, int> Limit;
    public readonly Dictionary<int, int> Tiers;

    public XenoEvolutionBoundInterfaceState(float evolution, float modifer, HashSet<XenoEvolution> evolutions, bool enabled, Dictionary<int, int> limit, Dictionary<int, int> tiers)
    {
        Evolution = evolution;
        EvolutionModifer = modifer;
        Evolutions = evolutions;
        Enabled = enabled;
        Limit = limit;
        Tiers = tiers;
    }
}

[Serializable, NetSerializable]
public sealed class EvolveMessage : BoundUserInterfaceMessage
{
    public readonly XenoEvolution Evolution;

    public EvolveMessage(XenoEvolution evolution)
    {
        Evolution = evolution;
    }
}

public sealed partial class XenoEvolutionActionEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public enum XenoEvolutionUiKey
{
    Key
}
