namespace Content.Shared.Xeno;
using Content.Shared.Actions;
using Robust.Shared.GameStates;

[RegisterComponent, NetworkedComponent]

public sealed partial class SharedDefilerAbilitiesComponent : Component { }
public sealed partial class DefilerDefaultSpitEvent : WorldTargetActionEvent { }

public sealed partial class DefilerAcidSpitEvent : WorldTargetActionEvent { }
public sealed partial class DefilerExplosiveEvent : InstantActionEvent { }
