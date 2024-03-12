using Content.Server.Shuttles.Systems;
using Content.Shared._CM14.EmergencyRetreat;
using Robust.Shared.Audio;
using Robust.Shared.Utility;

namespace Content.Server._CM14.EmergencyRetreat;

[RegisterComponent]
public sealed partial class EmergencyRetreatComponent : Component
{
    [DataField]
    public EntityUid? TargetFtlMapUid;

    [DataField]
    public ResPath TargetFtlMap = new("/Maps/Misc/terminal.yml");

    [DataField]
    public float FtlStartupTime = ShuttleSystem.DefaultStartupTime;

    [DataField]
    public float FtlHyperspaceTime = ShuttleSystem.DefaultTravelTime;

    [DataField]
    public bool FtlDock = true;

    [DataField]
    public EmergencyRetreatState State = EmergencyRetreatState.Idle;

    [DataField]
    public TimeSpan IdlePreparationDelay = TimeSpan.FromMinutes(5);

    [DataField]
    public TimeSpan IdlePreparationTime;

    [DataField]
    public TimeSpan FtlPreparationDelay = TimeSpan.FromMinutes(15);

    [DataField]
    public TimeSpan FtlPreparationTime;

    [DataField]
    public Color? AnnouncementColor = Color.FromHex("#56ffff");

    [DataField]
    public SoundSpecifier? AnnouncementSound;
}

