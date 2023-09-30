namespace Content.Shared.Xeno;
using Content.Shared.Actions;
using Robust.Shared.GameStates;

[RegisterComponent, NetworkedComponent]

public sealed partial class SharedDefilerAbilitiesComponent : Component { }
public sealed partial class DefilerSpitEvent : WorldTargetActionEvent { }
public sealed partial class DefilerExplosiveEvent : InstantActionEvent { }
