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

    public XenoEvolutionBoundInterfaceState(float evolution, float modifer, HashSet<XenoEvolution> evolutions, bool enabled)
    {
        Evolution = evolution;
        EvolutionModifer = modifer;
        Evolutions = evolutions;
        Enabled = enabled;
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
