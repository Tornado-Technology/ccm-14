using Robust.Shared.Serialization;

namespace Content.Shared._CM14.EmergencyRetreat;

[Serializable, NetSerializable]
public sealed class EmergencyRetreatBoundUserInterfaceState : BoundUserInterfaceState
{
    public readonly EmergencyRetreatState State;
    public readonly TimeSpan Time;

    public EmergencyRetreatBoundUserInterfaceState(EmergencyRetreatState state, TimeSpan time)
    {
        State = state;
        Time = time;
    }
}

[Serializable, NetSerializable]
public sealed class EmergencyRetreatRunFtlMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public enum EmergencyRetreatUiKey
{
    Key,
}
