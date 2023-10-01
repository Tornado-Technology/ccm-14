namespace Content.Shared.Xeno;
using Content.Shared.Actions;
using Robust.Shared.GameStates;

[RegisterComponent, NetworkedComponent]

public sealed partial class SharedSentinelAbilitiesComponent : Component { }
public sealed partial class SentinelDefaultSpitEvent : WorldTargetActionEvent { }
