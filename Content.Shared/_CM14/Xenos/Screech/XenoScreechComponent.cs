﻿using System.Numerics;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Xenos.Screech;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(XenoScreechSystem))]
public sealed partial class XenoScreechComponent : Component
{
    [DataField, AutoNetworkedField]
    public FixedPoint2 PlasmaCost = 250;
    
    [DataField, AutoNetworkedField]
    public TimeSpan StunTime = TimeSpan.FromSeconds(5);

    [DataField, AutoNetworkedField]
    public float StanRange = 10;

    [DataField, AutoNetworkedField]
    public float ParalyzeRange = 2;

    [DataField, AutoNetworkedField]
    public EntProtoId Effect = "CMEffectScreech";

    [DataField, AutoNetworkedField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/_CM/Xeno/alien_queen_screech.ogg");
}
