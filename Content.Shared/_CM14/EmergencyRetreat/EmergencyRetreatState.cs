using Robust.Shared.Serialization;

namespace Content.Shared._CM14.EmergencyRetreat;

[Serializable, NetSerializable]
public enum EmergencyRetreatState
{
    None,
    Idle,
    Ftl,
    FtlStarted,
    IdlePreparation,
    FtlPreparation,
}
