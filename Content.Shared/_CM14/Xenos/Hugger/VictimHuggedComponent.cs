﻿using Content.Shared._CM14.Medical.Surgery;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;
using static Robust.Shared.Utility.SpriteSpecifier;

namespace Content.Shared._CM14.Xenos.Hugger;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(SharedXenoHuggerSystem))]
public sealed partial class VictimHuggedComponent : Component
{
    [DataField, AutoNetworkedField]
    public string ContainerId = "cm_hugger_container";

    [DataField, AutoNetworkedField]
    public SpriteSpecifier HuggedSprite = new Rsi(new ResPath("/Textures/_CM/Mobs/Xenos/Hugger/hugger_mask.rsi"), "human");

    [DataField, AutoNetworkedField]
    public SpriteSpecifier[] HuggedIcons =
    [
        new Rsi(new ResPath("/Textures/_CM/Interface/xeno_hud.rsi"), "infected0"),
        new Rsi(new ResPath("/Textures/_CM/Interface/xeno_hud.rsi"), "infected1"),
        new Rsi(new ResPath("/Textures/_CM/Interface/xeno_hud.rsi"), "infected2"),
        new Rsi(new ResPath("/Textures/_CM/Interface/xeno_hud.rsi"), "infected3"),
        new Rsi(new ResPath("/Textures/_CM/Interface/xeno_hud.rsi"), "infected4"),
        new Rsi(new ResPath("/Textures/_CM/Interface/xeno_hud.rsi"), "infected5"),
        new Rsi(new ResPath("/Textures/_CM/Interface/xeno_hud.rsi"), "infected6")
    ];

    [DataField, AutoNetworkedField]
    public Enum HuggedLayer = VictimHuggedLayer.Hugged;

    [DataField, AutoNetworkedField]
    public TimeSpan FallOffDelay = TimeSpan.FromSeconds(10);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField, AutoPausedField]
    public TimeSpan FallOffAt;

    [DataField, AutoNetworkedField]
    public bool FellOff;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField, AutoPausedField]
    public TimeSpan RecoverAt;

    [DataField, AutoNetworkedField]
    public bool Recovered;

    [DataField, AutoNetworkedField]
    public TimeSpan BurstDelay = TimeSpan.FromMinutes(8);

    [DataField, AutoNetworkedField]
    public TimeSpan AttachedAt;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField, AutoPausedField]
    public TimeSpan BurstAt;

    [DataField, AutoNetworkedField]
    public float IncubationMultiplier = 1;

    [DataField, AutoNetworkedField]
    public EntProtoId BurstSpawn = "CMXenoLarva";

    [DataField, AutoNetworkedField]
    public SoundSpecifier BurstSound = new SoundCollectionSpecifier("XenoChestBurst");

    [DataField, AutoNetworkedField, Access(typeof(SharedCMSurgerySystem))]
    public bool RootsCut;
}
