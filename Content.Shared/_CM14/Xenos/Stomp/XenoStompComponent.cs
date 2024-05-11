﻿using System.Numerics;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Xenos.Stomp;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(XenoStompSystem))]
public sealed partial class XenoStompComponent : Component
{
    [DataField, AutoNetworkedField]
    public FixedPoint2 PlasmaCost = 20;

    [DataField]
    public DamageSpecifier Damage = new();

    [DataField, AutoNetworkedField]
    public TimeSpan ParalyzeTime = TimeSpan.FromSeconds(1);

    [DataField, AutoNetworkedField]
    public float ParalyzeRange = 2.5f;

    [DataField, AutoNetworkedField]
    public float DamageRange = 0.5f;

    [DataField, AutoNetworkedField]
    public EntProtoId SelfEffect = "CMEffectSelfStomp";

    [DataField, AutoNetworkedField]
    public EntProtoId Effect = "CMEffectStomp";

    [DataField, AutoNetworkedField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/_CM/Xeno/alien_footstep_charge1.ogg");
}
