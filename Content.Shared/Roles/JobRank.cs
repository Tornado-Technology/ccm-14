using Robust.Shared.Serialization;

namespace Content.Shared.Roles;

[Serializable, NetSerializable, DataDefinition]
public partial struct JobRank
{
    [DataField("time")]
    public TimeSpan Time;

    [DataField("prefix")]
    public string? Prefix;
}
