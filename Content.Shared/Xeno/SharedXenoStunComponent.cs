using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System.Numerics;

namespace Content.Shared.Xeno;

[RegisterComponent, NetworkedComponent]
public sealed partial class SharedXenoStunAbilitiesComponent : Component { }
public sealed partial class XenoStunEvent : EntityTargetActionEvent { }
public sealed partial class XenoExplosiveEvent : InstantActionEvent { }
public sealed partial class XenoSpitEvent : WorldTargetActionEvent { }
public sealed partial class XenoSpit2Event : WorldTargetActionEvent { }
public sealed partial class XenoCrushDashEvent : WorldTargetActionEvent { }
public sealed partial class XenoVinesEvent : InstantActionEvent { }
public sealed partial class XenoToggleStealthEvent: InstantActionEvent { }

// Sentinel
public sealed partial class XenoDroneBuildEvent : WorldTargetActionEvent { }
public sealed partial class XenoDronePsychicCureEvent : EntityTargetActionEvent { }
public sealed partial class XenoDroneVinesEvent : InstantActionEvent { }
[Serializable, NetSerializable]
public sealed partial class XenoDronePsychicCureDoAfterEvent : SimpleDoAfterEvent { }
[Serializable, NetSerializable]
public sealed partial class XenoDroneBuildDoAfterEvent : SimpleDoAfterEvent
{
    public readonly MapCoordinates Coordinates;

    public XenoDroneBuildDoAfterEvent(MapCoordinates coordinates)
    {
        Coordinates = coordinates;
    }
}

// Hivelord
public sealed partial class XenoHivelordBuildEvent : WorldTargetActionEvent { }
public sealed partial class XenoHivelordPsychicCureEvent : EntityTargetActionEvent { }
[Serializable, NetSerializable]
public sealed partial class XenoHivelordPsychicCureDoAfterEvent : SimpleDoAfterEvent { }
[Serializable, NetSerializable]
public sealed partial class XenoHivelordBuildDoAfterEvent : SimpleDoAfterEvent
{
    public readonly MapCoordinates Coordinates;

    public XenoHivelordBuildDoAfterEvent(MapCoordinates coordinates)
    {
        Coordinates = coordinates;
    }
}
